using DatingApp.Contracts.Common;
using DatingApp.Contracts.Likes.Requests;
using DatingApp.Contracts.Members.Responses;

namespace DatingApp.Api.Features.Likes;

public interface ILikesService
{
    Task<PaginatedResponse<IEnumerable<MemberResponse>>> GetUserLikesAsync(string requestor, int userId, LikesListQuery query);
    Task<ApiResponse<string>> ToggleLikeAsync(string requestor, string username, int sourceUserId);
}
