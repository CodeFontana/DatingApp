namespace DataAccessLibrary.Interfaces;

public interface IMessageRepository
{
    void AddMessage(Message message);
    void DeleteMessage(Message message);
    Task<Message> GetMessageAsync(int id);
    Task<PaginationResponseModel<PaginationList<MessageModel>>> GetMessagesForMember();
    Task<PaginationResponseModel<PaginationList<MessageModel>>> GetMessageThread();
    Task<bool> SaveAllAsync();
}