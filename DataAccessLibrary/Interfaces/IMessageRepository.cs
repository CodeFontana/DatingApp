namespace DataAccessLibrary.Interfaces;

public interface IMessageRepository
{
    Task CreateMessageAsync(Message message);
    Task<Message> GetMessageAsync(int id);
    Task<PaginationList<MessageModel>> GetMessagesForMemberAsync(MessageParameters messageParameters);
    Task<IEnumerable<MessageModel>> GetMessageThreadAsync(string currentUsername, string recipientUsername);
    Task<Tuple<string, string>> DeleteMessageAsync(string requestUser, int id);
    Task<bool> SaveAllAsync();
}