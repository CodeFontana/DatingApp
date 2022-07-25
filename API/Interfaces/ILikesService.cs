namespace API.Interfaces;

public interface ILikesService
{
    Task<PaginationResponseModel<PaginationList<LikeUserModel>>> GetUserLikesAsync(string requestor, LikesParameters likesParameters);
    Task<ServiceResponseModel<string>> ToggleLikeAsync(string reqeustor, string username, int sourceUserId);
}