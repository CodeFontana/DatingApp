using DatingApp.Api.Features.Messages;
using DatingApp.Api.Infrastructure.Hubs;
using DatingApp.Contracts.Messages.Requests;
using DatingApp.Contracts.Messages.Responses;
using DatingApp.DataAccess.Entities;
using DatingApp.DataAccess.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.Api.Features.Messages;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class MessageHub : Hub
{
    private readonly ILogger<MessageHub> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPresenceTrackerService _presenceTrackerService;
    private readonly IHubContext<PresenceHub> _presenceHub;

    public MessageHub(
        ILogger<MessageHub> logger,
        IUnitOfWork unitOfWork,
        IPresenceTrackerService presenceTrackerService,
        IHubContext<PresenceHub> presenceHub)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _presenceTrackerService = presenceTrackerService;
        _presenceHub = presenceHub;
    }

    public override async Task OnConnectedAsync()
    {
        string otherUser = Context.GetHttpContext()!.Request.Query["user"].ToString();
        string username = Context.User!.Identity!.Name!;
        string groupName = GetGroupName(username, otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        IEnumerable<MessageResponse> messages = (await _unitOfWork.MessageRepository.GetMessageThreadAsync(username, otherUser))
            .Select(MessageMapper.ToResponse);

        if (_unitOfWork.HasChanges())
        {
            await _unitOfWork.CompleteAsync();
        }

        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
        _logger.LogInformation("User {Username} connected, established {GroupName}", username, groupName);
    }

    public async Task SendMessage(CreateMessageRequest messageCreateModel)
    {
        string username = Context.User!.Identity!.Name!;

        if (username.Equals(messageCreateModel.RecipientUsername, StringComparison.OrdinalIgnoreCase))
        {
            throw new HubException("You cannot send messages to yourself");
        }

        AppUser sender = await _unitOfWork.MemberRepository.GetMemberByUsernameAsync(username)
            ?? throw new HubException($"Sender not found [{username}]");
        AppUser recipient = await _unitOfWork.MemberRepository.GetMemberByUsernameAsync(messageCreateModel.RecipientUsername)
            ?? throw new HubException($"Recipient not found [{messageCreateModel.RecipientUsername}]");

        Message message = new()
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName ?? string.Empty,
            RecipientUsername = recipient.UserName ?? string.Empty,
            Content = messageCreateModel.Content
        };

        await _unitOfWork.MessageRepository.CreateMessageAsync(message);
        await _unitOfWork.CompleteAsync();

        string group = GetGroupName(sender.UserName!, recipient.UserName!);
        await Clients.Group(group).SendAsync("ReceiveMessage", MessageMapper.ToResponse(message));

        string[] onlineUsers = await _presenceTrackerService.GetOnlineUsers();

        if (onlineUsers.Contains(recipient.UserName))
        {
            await _presenceHub.Clients.User(recipient.UserName!).SendAsync("MessageReceived", sender.KnownAs);
        }

        _logger.LogInformation("User {Username} sent message to {Recipient}", username, messageCreateModel.RecipientUsername);
    }

    public async Task SendThreadAck(DateTime ackTime, string otherUser)
    {
        await Clients.User(otherUser).SendAsync("ReceiveThreadAck", ackTime);
        _logger.LogInformation("User {Username} acknowledged thread to {OtherUser} at {AckTime}", Context.User!.Identity!.Name, otherUser, ackTime.ToLocalTime());
    }

    private static string GetGroupName(string caller, string other)
    {
        return string.CompareOrdinal(caller, other) < 0 ? $"{caller}-{other}" : $"{other}-{caller}";
    }
}
