using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Hubs;
using API.Interfaces;
using DataAccessLibrary.Entities;
using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;
using DataAccessLibrary.Pagination;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace API.Services;

public sealed class MessageService : IMessageService
{
    private readonly IPresenceTrackerService _presenceTrackerService;
    private readonly IHubContext<PresenceHub> _presenceHub;
    private readonly ILogger<MessageService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public MessageService(ILogger<MessageService> logger,
                          IUnitOfWork unitOfWork,
                          IPresenceTrackerService presenceTrackerService,
                          IHubContext<PresenceHub> presenceHub)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _presenceTrackerService = presenceTrackerService;
        _presenceHub = presenceHub;
    }

    public async Task<ServiceResponseModel<MessageModel>> CreateMessageAsync(string requestor, MessageCreateModel messageCreateModel)
    {
        _logger.LogInformation($"Create message for {messageCreateModel.RecipientUsername}... [{requestor}]");
        ServiceResponseModel<MessageModel> serviceResponse = new();

        try
        {
            if (requestor.ToLower() == messageCreateModel.RecipientUsername.ToLower())
            {
                throw new ArgumentException("You cannot send messages to yourself");
            }

            AppUser sender = await _unitOfWork.MemberRepository.GetMemberByUsernameAsync(requestor);
            AppUser recipent = await _unitOfWork.MemberRepository.GetMemberByUsernameAsync(messageCreateModel.RecipientUsername);

            if (recipent is null)
            {
                throw new ArgumentException($"Recipient not found [{messageCreateModel.RecipientUsername}]");
            }

            Message message = new()
            {
                Sender = sender,
                Recipient = recipent,
                SenderUsername = sender.UserName,
                RecipientUsername = recipent.UserName,
                Content = messageCreateModel.Content
            };

            await _unitOfWork.MessageRepository.CreateMessageAsync(message);

            if (await _unitOfWork.CompleteAsync())
            {
                string[] onlineUsers = await _presenceTrackerService.GetOnlineUsers();

                if (onlineUsers.Contains(recipent.UserName))
                {
                    await _presenceHub.Clients.User(recipent.UserName).SendAsync("MessageReceived", sender.KnownAs);
                }

                serviceResponse.Success = true;
                serviceResponse.Data = MessageModel.FromEntity(message);
                serviceResponse.Message = $"Successfully created message from {sender.UserName} to {recipent.UserName}";
                _logger.LogInformation(serviceResponse.Message);
            }
            else
            {
                throw new Exception($"Failed to send message from {requestor} to {messageCreateModel.RecipientUsername}");
            }
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
            _logger.LogError(e.Message);
        }

        return serviceResponse;
    }

    public async Task<PaginationResponseModel<PaginationList<MessageModel>>> GetMessagesForMemberAsync(string requestor, MessageParameters messageParameters)
    {
        _logger.LogInformation($"Get messages for {messageParameters.Username}... [{requestor}]");
        PaginationResponseModel<PaginationList<MessageModel>> pagedResponse = new();

        try
        {
            PaginationList<MessageModel> data = await _unitOfWork.MessageRepository.GetMessagesForMemberAsync(messageParameters);

            pagedResponse.Success = true;
            pagedResponse.Data = data;
            pagedResponse.MetaData = data.MetaData;
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

    public async Task<ServiceResponseModel<IEnumerable<MessageModel>>> GetMessageThreadAsync(string currentUsername, string recipientUsername)
    {
        _logger.LogInformation($"Get message thread between sender {currentUsername} and {recipientUsername}...");
        ServiceResponseModel<IEnumerable<MessageModel>> serviceResponse = new();

        try
        {
            IEnumerable<MessageModel> messages = await _unitOfWork.MessageRepository.GetMessageThreadAsync(currentUsername, recipientUsername);

            if (_unitOfWork.HasChanges())
            {
                await _unitOfWork.CompleteAsync();
            }

            serviceResponse.Success = true;
            serviceResponse.Data = messages;
            serviceResponse.Message = $"Successfully retrieved message thread between {currentUsername} and {recipientUsername}";
            _logger.LogInformation(serviceResponse.Message);
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
            _logger.LogError(e.Message);
        }

        return serviceResponse;
    }

    public async Task<ServiceResponseModel<string>> DeleteMessageAsync(string requestor, int messageId)
    {
        _logger.LogInformation($"Delete message with id={messageId}... [{requestor}]");
        ServiceResponseModel<string> serviceResponse = new();

        try
        {
            Tuple<string, string> msgInfo = _unitOfWork.MessageRepository.DeleteMessageAsync(requestor, messageId);

            if (_unitOfWork.HasChanges())
            {
                await _unitOfWork.CompleteAsync();
            }

            serviceResponse.Success = true;
            serviceResponse.Data = $"Successfully deleted message from {msgInfo.Item1} to {msgInfo.Item2}";
            serviceResponse.Message = $"Successfully deleted message [{requestor}]";
            _logger.LogInformation(serviceResponse.Message);
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
            _logger.LogError(e.Message);
        }

        return serviceResponse;
    }
}
