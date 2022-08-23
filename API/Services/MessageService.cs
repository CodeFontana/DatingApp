namespace API.Services;

public class MessageService : IMessageService
{
	private readonly IMemberRepository _memberRepository;
	private readonly IMessageRepository _messageRepository;
	private readonly IMapper _mapper;
	private readonly ILogger<MessageService> _logger;

	public MessageService(IMemberRepository memberRepository,
						  IMessageRepository messageRepository,
						  IMapper mapper,
						  ILogger<MessageService> logger)
	{
		_memberRepository = memberRepository;
		_messageRepository = messageRepository;
		_mapper = mapper;
		_logger = logger;
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

			AppUser sender = await _memberRepository.GetMemberByUsernameAsync(requestor);
			AppUser recipent = await _memberRepository.GetMemberByUsernameAsync(messageCreateModel.RecipientUsername);

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

			await _messageRepository.CreateMessageAsync(message);

			if (await _messageRepository.SaveAllAsync())
			{
				serviceResponse.Success = true;
				serviceResponse.Data = _mapper.Map<MessageModel>(message);
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
            PaginationList<MessageModel> data = await _messageRepository.GetMessagesForMemberAsync(messageParameters);

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
			serviceResponse.Success = true;
			serviceResponse.Data = await _messageRepository.GetMessageThreadAsync(currentUsername, recipientUsername);
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
            await _messageRepository.DeleteMessageAsync(requestor, messageId);

            serviceResponse.Success = true;
			serviceResponse.Data = $"Successfully deleted message";
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
