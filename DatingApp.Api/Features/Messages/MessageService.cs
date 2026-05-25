using DatingApp.Api.Features.Common;
using DatingApp.Api.Infrastructure.Hubs;
using DatingApp.Contracts.Common;
using DatingApp.Contracts.Messages.Requests;
using DatingApp.Contracts.Messages.Responses;
using DatingApp.DataAccess.Entities;
using DatingApp.DataAccess.Interfaces;
using DatingApp.DataAccess.Internal;
using DatingApp.DataAccess.Pagination;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.Api.Features.Messages;

public sealed class MessageService : IMessageService
{
    private readonly IPresenceTrackerService _presenceTrackerService;
    private readonly IHubContext<PresenceHub> _presenceHub;
    private readonly ILogger<MessageService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public MessageService(
        ILogger<MessageService> logger,
        IUnitOfWork unitOfWork,
        IPresenceTrackerService presenceTrackerService,
        IHubContext<PresenceHub> presenceHub)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _presenceTrackerService = presenceTrackerService;
        _presenceHub = presenceHub;
    }

    public async Task<ApiResponse<MessageResponse>> CreateMessageAsync(string requestor, CreateMessageRequest messageCreate)
    {
        _logger.LogInformation("Create message for {Recipient}... [{Requestor}]", messageCreate.RecipientUsername, requestor);
        ApiResponse<MessageResponse> response = new();

        try
        {
            if (requestor.Equals(messageCreate.RecipientUsername, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("You cannot send messages to yourself");
            }

            AppUser sender = await _unitOfWork.MemberRepository.GetMemberByUsernameAsync(requestor)
                ?? throw new ArgumentException($"Sender not found [{requestor}]");
            AppUser recipient = await _unitOfWork.MemberRepository.GetMemberByUsernameAsync(messageCreate.RecipientUsername)
                ?? throw new ArgumentException($"Recipient not found [{messageCreate.RecipientUsername}]");

            Message message = new()
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName ?? string.Empty,
                RecipientUsername = recipient.UserName ?? string.Empty,
                Content = messageCreate.Content
            };

            await _unitOfWork.MessageRepository.CreateMessageAsync(message);

            if (await _unitOfWork.CompleteAsync())
            {
                string[] onlineUsers = await _presenceTrackerService.GetOnlineUsers();

                if (onlineUsers.Contains(recipient.UserName))
                {
                    await _presenceHub.Clients.User(recipient.UserName!).SendAsync("MessageReceived", sender.KnownAs);
                }

                response.Success = true;
                response.Data = MessageMapper.ToResponse(message);
                response.Message = $"Successfully created message from {sender.UserName} to {recipient.UserName}";
                _logger.LogInformation(response.Message);
            }
            else
            {
                throw new Exception($"Failed to send message from {requestor} to {messageCreate.RecipientUsername}");
            }
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
            _logger.LogError(e.Message);
        }

        return response;
    }

    public async Task<PaginatedResponse<IEnumerable<MessageResponse>>> GetMessagesForMemberAsync(string requestor, MessageListQuery query)
    {
        _logger.LogInformation("Get messages for {Requestor}...", requestor);
        PaginatedResponse<IEnumerable<MessageResponse>> pagedResponse = new();

        try
        {
            PaginationList<MessageReadModel> data = await _unitOfWork.MessageRepository.GetMessagesForMemberAsync(
                MessageMapper.ToCriteria(query, requestor));

            pagedResponse.Success = true;
            pagedResponse.Data = data.Select(MessageMapper.ToResponse);
            pagedResponse.MetaData = PaginationMapper.ToContract(data.MetaData);
            pagedResponse.Message = $"Successfully listed messages for [{requestor}]";
            _logger.LogInformation(pagedResponse.Message);
        }
        catch (Exception e)
        {
            pagedResponse.Success = false;
            pagedResponse.Message = e.Message;
            _logger.LogError(e.Message);
        }

        return pagedResponse;
    }

    public async Task<ApiResponse<IEnumerable<MessageResponse>>> GetMessageThreadAsync(string currentUsername, string recipientUsername)
    {
        _logger.LogInformation("Get message thread between {Current} and {Recipient}...", currentUsername, recipientUsername);
        ApiResponse<IEnumerable<MessageResponse>> response = new();

        try
        {
            IEnumerable<MessageReadModel> messages = await _unitOfWork.MessageRepository.GetMessageThreadAsync(
                currentUsername, recipientUsername);

            if (_unitOfWork.HasChanges())
            {
                await _unitOfWork.CompleteAsync();
            }

            response.Success = true;
            response.Data = messages.Select(MessageMapper.ToResponse);
            response.Message = $"Successfully retrieved message thread between {currentUsername} and {recipientUsername}";
            _logger.LogInformation(response.Message);
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
            _logger.LogError(e.Message);
        }

        return response;
    }

    public async Task<ApiResponse<string>> DeleteMessageAsync(string requestor, int messageId)
    {
        _logger.LogInformation("Delete message with id={MessageId}... [{Requestor}]", messageId, requestor);
        ApiResponse<string> response = new();

        try
        {
            Tuple<string, string> msgInfo = _unitOfWork.MessageRepository.DeleteMessageAsync(requestor, messageId);

            if (_unitOfWork.HasChanges())
            {
                await _unitOfWork.CompleteAsync();
            }

            response.Success = true;
            response.Data = $"Successfully deleted message from {msgInfo.Item1} to {msgInfo.Item2}";
            response.Message = $"Successfully deleted message [{requestor}]";
            _logger.LogInformation(response.Message);
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
            _logger.LogError(e.Message);
        }

        return response;
    }
}
