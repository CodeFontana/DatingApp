namespace API.Hubs;

[Authorize]
public class MessageHub : Hub
{
	private readonly IMessageRepository _messageRepository;
	private readonly IMemberRepository _memberRepository;
	private readonly IMapper _mapper;

	public MessageHub(IMessageRepository messageRepository,
					  IMemberRepository memberRepository,
					  IMapper mapper)
	{
		_messageRepository = messageRepository;
		_memberRepository = memberRepository;
		_mapper = mapper;
	}

	public override async Task OnConnectedAsync()
	{
		HttpContext httpContext = Context.GetHttpContext();
		string otherUser = httpContext.Request.Query["user"].ToString();
		string groupName = GetGroupName(Context.User.Identity.Name, otherUser);
		await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
		IEnumerable<MessageModel> messages = await _messageRepository.GetMessageThreadAsync(Context.User.Identity.Name, otherUser);
		await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
	}

	public override async Task OnDisconnectedAsync(Exception exception)
	{
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
    }

	private string GetGroupName(string caller, string other)
	{
		bool stringCompare = string.CompareOrdinal(caller, other) < 0;
		return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
	}
}
