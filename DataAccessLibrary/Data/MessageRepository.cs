namespace DataAccessLibrary.Data;

public class MessageRepository : IMessageRepository
{
    private readonly DataContext _db;
    private readonly IMapper _mapper;

    public MessageRepository(DataContext context, IMapper mapper)
    {
        _db = context;
        _mapper = mapper;
    }

    public async Task CreateMessageAsync(Message message)
    {
        await _db.Messages.AddAsync(message);
    }

    public void DeleteMessage(Message message)
    {
        _db.Messages.Remove(message);
    }

    public async Task<Message> GetMessageAsync(int id)
    {
        return await _db.Messages.FindAsync(id);
    }

    public async Task<PaginationList<MessageModel>> GetMessagesForMemberAsync(MessageParameters messageParameters)
    {
        var query = _db.Messages
            .OrderByDescending(m => m.MessageSent)
            .AsQueryable();

        query = messageParameters.Container switch
        {
            "Inbox" => query.Where(u => u.Recipient.UserName == messageParameters.Username),
            "Sent" => query.Where(u => u.Sender.UserName == messageParameters.Username),
            _ => query.Where(u => u.Recipient.UserName == messageParameters.Username && u.DateRead == null)
        };

        IQueryable<MessageModel> messages = query.ProjectTo<MessageModel>(_mapper.ConfigurationProvider);

        return await PaginationList<MessageModel>.CreateAsync(
            messages,
            messageParameters.PageNumber,
            messageParameters.PageSize);
    }

    public async Task<IEnumerable<MessageModel>> GetMessageThreadAsync(string currentUsername, string recipientUsername)
    {
        List<Message> messages = await _db.Messages
            .Include(u => u.Sender).ThenInclude(p => p.Photos)
            .Include(u => u.Recipient).ThenInclude(p => p.Photos)
            .Where(m => m.Recipient.UserName == currentUsername 
                && m.Sender.UserName == recipientUsername
                || m.Recipient.UserName == recipientUsername
                && m.Sender.UserName == currentUsername)
            .OrderBy(m => m.MessageSent)
            .AsSplitQuery()
            .ToListAsync();

        List<Message> unreadMessages = messages.Where(m => m.DateRead == null && m.Recipient.UserName == currentUsername).ToList();

        if (unreadMessages.Any())
        {
            foreach (var message in unreadMessages)
            {
                message.DateRead = DateTime.Now;
            }

            await _db.SaveChangesAsync();
        }
        
        return _mapper.Map<IEnumerable<MessageModel>>(messages);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _db.SaveChangesAsync() > 0;
    }
}
