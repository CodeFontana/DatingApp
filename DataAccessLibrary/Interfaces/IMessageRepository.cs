namespace DataAccessLibrary.Interfaces;

public interface IMessageRepository
{
    Task CreateMessageAsync(Message message);
    Task DeleteMessageAsync(string requestUser, int id);
    Task<Message> GetMessageAsync(int id);
    Task<PaginationList<MessageModel>> GetMessagesForMemberAsync(MessageParameters messageParameters);
    Task<IEnumerable<MessageModel>> GetMessageThreadAsync(string currentUsername, string recipientUsername);
    Task<bool> SaveAllAsync();
}