namespace DatingApp.Client.Services;

public interface ILikesService
{
    Dictionary<string, MemberCacheResponse> LikeListCache { get; set; }
    Task<PaginatedResponse<IEnumerable<MemberResponse>>> GetLikesAsync(LikesListQuery likesParameters);
    Task<ApiResponse<string>> ToggleLikeAsync(string username);
}