namespace API.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class MessageHub : Hub
{
	private readonly ILogger<MessageHub> _logger;
	private readonly IMessageRepository _messageRepository;
	private readonly IMemberRepository _memberRepository;
	private readonly IPresenceTrackerService _presenceTrackerService;
	private readonly IHubContext<PresenceHub> _presenceHub;
	private readonly IMapper _mapper;

	public MessageHub(ILogger<MessageHub> logger,
					  IMessageRepository messageRepository,
					  IMemberRepository memberRepository,
					  IPresenceTrackerService presenceTrackerService,
					  IHubContext<PresenceHub> presenceHub,
					  IMapper mapper)
	{
		_logger = logger;
		_messageRepository = messageRepository;
		_memberRepository = memberRepository;
		_presenceTrackerService = presenceTrackerService;
		_presenceHub = presenceHub;
		_mapper = mapper;
	}

	public override async Task OnConnectedAsync()
	{
		HttpContext httpContext = Context.GetHttpContext();
		string otherUser = httpContext.Request.Query["user"].ToString();
		string groupName = GetGroupName(Context.User.Identity.Name, otherUser);
		await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
		IEnumerable<MessageModel> messages = await _messageRepository.GetMessageThreadAsync(Context.User.Identity.Name, otherUser);
		await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
		_logger.LogInformation($"User {Context.User.Identity.Name} connected, established {groupName}");
	}

	public override async Task OnDisconnectedAsync(Exception exception)
	{
        _logger.LogInformation($"User {Context.User.Identity.Name} disconnected");
        await base.OnDisconnectedAsync(exception);
	}

	public async Task SendMessage(MessageCreateModel messageCreateModel)
	{
        if (Context.User.Identity.Name.ToLower() == messageCreateModel.RecipientUsername.ToLower())
        {
            throw new HubException("You cannot send messages to yourself");
        }

        AppUser sender = await _memberRepository.GetMemberByUsernameAsync(Context.User.Identity.Name);
        AppUser recipent = await _memberRepository.GetMemberByUsernameAsync(messageCreateModel.RecipientUsername);

        if (recipent is null)
        {
            throw new HubException($"Recipient not found [{messageCreateModel.RecipientUsername}]");
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
		await _messageRepository.SaveAllAsync();
		
		string group = GetGroupName(sender.UserName, recipent.UserName);
		await Clients.Group(group).SendAsync("ReceiveMessage", _mapper.Map<MessageModel>(message));

        string[] onlineUsers = await _presenceTrackerService.GetOnlineUsers();

        if (onlineUsers.Contains(recipent.UserName))
        {
            await _presenceHub.Clients.User(recipent.UserName).SendAsync("MessageReceived", sender.KnownAs);
        }

		_logger.LogInformation($"User {Context.User.Identity.Name} sent message to {messageCreateModel.RecipientUsername}");
    }

	public async Task SendThreadAck(DateTime ackTime, string otherUser)
	{
		await Clients.User(otherUser).SendAsync("ReceiveThreadAck", ackTime);
        _logger.LogInformation($"User {Context.User.Identity.Name} acknowledged thread to {otherUser} at {ackTime.ToLocalTime()}");
    }

	private string GetGroupName(string caller, string other)
	{
		bool stringCompare = string.CompareOrdinal(caller, other) < 0;
		return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
	}
}
