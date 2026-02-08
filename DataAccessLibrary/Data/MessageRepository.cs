namespace DataAccessLibrary.Data;

public class MessageRepository : IMessageRepository
{
    private readonly DataContext _db;

    public MessageRepository(DataContext context)
    {
        _db = context;
    }

    public async Task CreateMessageAsync(Message message)
    {
        await _db.Messages.AddAsync(message);
    }

    public async Task<Message> GetMessageAsync(int id)
    {
        return await _db.Messages.FindAsync(id);
    }

    public async Task<PaginationList<MessageModel>> GetMessagesForMemberAsync(MessageParameters messageParameters)
    {
        IQueryable<MessageModel> messages = _db.Messages
            .OrderByDescending(m => m.MessageSent)
            .Select(MessageModel.Projection)
            .AsQueryable();

        messages = messageParameters.Container switch
        {
            "Inbox" => messages.Where(u => u.RecipientUsername == messageParameters.Username && u.RecipientDeleted == false),
            "Sent" => messages.Where(u => u.SenderUsername == messageParameters.Username && u.SenderDeleted == false),
            _ => messages.Where(u => u.RecipientUsername == messageParameters.Username && u.DateRead == null && u.RecipientDeleted == false)
        };

        return await PaginationList<MessageModel>.CreateAsync(
            messages,
            messageParameters.PageNumber,
            messageParameters.PageSize);
    }

    public async Task<IEnumerable<MessageModel>> GetMessageThreadAsync(string currentUsername, string recipientUsername)
    {
        List<MessageModel> messages = await _db.Messages
            .Where(m => m.Recipient.UserName == currentUsername
                && m.Sender.UserName == recipientUsername
                || m.Recipient.UserName == recipientUsername
                && m.Sender.UserName == currentUsername)
            .OrderByDescending(m => m.MessageSent)
            .AsSplitQuery()
            .Take(10)
            .Reverse()
            .Select(MessageModel.Projection)
            .ToListAsync();

        List<MessageModel> unreadMessages = messages
            .Where(m => m.DateRead == null && m.RecipientUsername == currentUsername).ToList();

        if (unreadMessages.Any())
        {
            foreach (MessageModel message in unreadMessages)
            {
                message.DateRead = DateTime.UtcNow;
            }
        }

        return messages;
    }

    public Tuple<string, string> DeleteMessageAsync(string requestUser, int id)
    {
        Message message = _db.Messages.FirstOrDefault(m => m.Id == id);

        if (message is null)
        {
            throw new ArgumentException($"Message with id={id} was not found");
        }

        if (message.SenderUsername != requestUser
            && message.RecipientUsername != requestUser)
        {
            throw new UnauthorizedAccessException($"Request user [{requestUser}] is not authorized to delete this message");
        }

        if (message.SenderUsername == requestUser)
        {
            message.SenderDeleted = true;
        }

        if (message.RecipientUsername == requestUser)
        {
            message.RecipientDeleted = true;
        }

        if (message.SenderDeleted && message.RecipientDeleted)
        {
            _db.Messages.Remove(message);
        }

        return new Tuple<string, string>(message.SenderUsername, message.RecipientUsername);
    }

}
