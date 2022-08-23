using System.Security.Cryptography.X509Certificates;

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

    public async Task<Message> GetMessageAsync(int id)
    {
        return await _db.Messages.FindAsync(id);
    }

    public async Task<PaginationList<MessageModel>> GetMessagesForMemberAsync(MessageParameters messageParameters)
    {
        IQueryable<Message> query = _db.Messages
            .OrderByDescending(m => m.MessageSent)
            .AsQueryable();

        query = messageParameters.Container switch
        {
            "Inbox" => query.Where(u => u.Recipient.UserName == messageParameters.Username && u.RecipientDeleted == false),
            "Sent" => query.Where(u => u.Sender.UserName == messageParameters.Username && u.SenderDeleted == false),
            _ => query.Where(u => u.Recipient.UserName == messageParameters.Username && u.DateRead == null && u.RecipientDeleted == false)
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
            .OrderByDescending(m => m.MessageSent)
            .Take(10)
            .Reverse()
            .AsSplitQuery()
            .ToListAsync();

        List<Message> unreadMessages = messages
            .Where(m => m.DateRead == null && m.Recipient.UserName == currentUsername).ToList();

        if (unreadMessages.Any())
        {
            foreach (Message message in unreadMessages)
            {
                message.DateRead = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
        }
        
        return _mapper.Map<IEnumerable<MessageModel>>(messages);
    }

    public async Task DeleteMessageAsync(string requestUser, int id)
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

        await _db.SaveChangesAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _db.SaveChangesAsync() > 0;
    }
}
