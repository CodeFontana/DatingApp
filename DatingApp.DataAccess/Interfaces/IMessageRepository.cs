namespace DatingApp.DataAccess.Interfaces;

public interface IMessageRepository
{
    Task CreateMessageAsync(Message message);
    Task<Message?> GetMessageAsync(int id);
    Task<PaginationList<MessageReadModel>> GetMessagesForMemberAsync(MessageListCriteria criteria);
    Task<IEnumerable<MessageReadModel>> GetMessageThreadAsync(string currentUsername, string recipientUsername);
    Tuple<string, string> DeleteMessageAsync(string requestUser, int id);
}
