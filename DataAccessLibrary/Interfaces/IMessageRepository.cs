namespace DataAccessLibrary.Interfaces;

public interface IMessageRepository
{
    Task CreateMessageAsync(Message message);
    void DeleteMessage(Message message);
    Task<Message> GetMessageAsync(int id);
    Task<PaginationList<MessageModel>> GetMessagesForMemberAsync(MessageParameters messageParameters);
    Task<IEnumerable<MessageModel>> GetMessageThreadAsync(string currentUsername, string recipientUsername);
    Task<bool> SaveAllAsync();
}