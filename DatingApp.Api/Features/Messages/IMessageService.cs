using DatingApp.Contracts.Common;
using DatingApp.Contracts.Messages.Requests;
using DatingApp.Contracts.Messages.Responses;

namespace DatingApp.Api.Features.Messages;

public interface IMessageService
{
    Task<ApiResponse<MessageResponse>> CreateMessageAsync(string requestor, CreateMessageRequest messageCreate);
    Task<PaginatedResponse<IEnumerable<MessageResponse>>> GetMessagesForMemberAsync(string requestor, MessageListQuery query);
    Task<ApiResponse<IEnumerable<MessageResponse>>> GetMessageThreadAsync(string currentUsername, string recipientUsername);
    Task<ApiResponse<string>> DeleteMessageAsync(string requestor, int messageId);
}
