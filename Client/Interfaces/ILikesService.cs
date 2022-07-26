namespace Client.Interfaces;

public interface ILikesService
{
    Dictionary<string, MemberCacheModel> LikeListCache { get; set; }
    LikesParameters LikesFilter { get; set; }
    Task<PaginationResponseModel<IEnumerable<MemberModel>>> GetLikesAsync(LikesParameters likesParameters);
    Task<ServiceResponseModel<string>> ToggleLikeAsync(string username);
}