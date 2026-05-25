namespace DatingApp.Client.Services;

public interface IMessageService
{
    List<MessageResponse> Messages { get; set; }
    bool ConnectedToHub { get; }

    event Action MessagesChanged;

    Task ConnectAsync(string jwtToken, string otherUser);
    Task CreateHubMessageAsync(CreateMessageRequest messageCreateModel);
    Task DisconnectAsync();
    Task<ApiResponse<MessageResponse>> CreateMessageAsync(CreateMessageRequest messageCreateModel);
    Task<ApiResponse<string>> DeleteMessageAsync(int id);
    Task<PaginatedResponse<List<MessageResponse>>> GetMessagesForMemberAsync(MessageListQuery messageParameters);
    Task<ApiResponse<List<MessageResponse>>> GetMessageThreadAsync(string username);
}