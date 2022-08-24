namespace Client.Interfaces;

public interface IMessageService
{
    List<MessageModel> Messages { get; set; }
    bool ConnectedToHub { get; }

    event Action MessagesChanged;

    Task ConnectAsync(string jwtToken, string otherUser);
    Task DisconnectAsync();
    Task<ServiceResponseModel<MessageModel>> CreateMessageAsync(MessageCreateModel messageCreateModel);
    Task<ServiceResponseModel<string>> DeleteMessageAsync(int id);
    Task<PaginationResponseModel<List<MessageModel>>> GetMessagesForMemberAsync(MessageParameters messageParameters);
    Task<ServiceResponseModel<List<MessageModel>>> GetMessageThreadAsync(string username);
    Task CreateHubMessageAsync(MessageCreateModel messageCreateModel);
}