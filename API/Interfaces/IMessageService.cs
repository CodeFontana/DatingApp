namespace API.Interfaces;

public interface IMessageService
{
    Task<ServiceResponseModel<MessageModel>> CreateMessageAsync(string requestor, MessageCreateModel messageCreateModel);
    Task<PaginationResponseModel<PaginationList<MessageModel>>> GetMessagesForMemberAsync(string requestor, MessageParameters messageParameters);
    Task<ServiceResponseModel<IEnumerable<MessageModel>>> GetMessageThreadAsync(string currentUsername, string recipientUsername);
    Task<ServiceResponseModel<string>> DeleteMessageAsync(string requestor, int messageId);
}