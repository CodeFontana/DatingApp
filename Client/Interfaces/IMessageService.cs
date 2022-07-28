namespace Client.Interfaces;

public interface IMessageService
{
    Task<ServiceResponseModel<MessageModel>> CreateMessageAsync(MessageCreateModel messageCreateModel);
    Task<PaginationResponseModel<IEnumerable<MessageModel>>> GetMessagesForMemberAsync(MessageParameters messageParameters);
    Task<ServiceResponseModel<IEnumerable<MessageModel>>> GetMessageThreadAsync(string username);
}