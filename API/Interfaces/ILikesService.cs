namespace API.Interfaces;

public interface ILikesService
{
    Task<ServiceResponseModel<string>> ToggleLikeAsync(string reqeustor, string username, int sourceUserId);
    Task<ServiceResponseModel<IEnumerable<LikeUserModel>>> GetUserLikesAsync(string requestor, string predicate, int sourceUserId);
}