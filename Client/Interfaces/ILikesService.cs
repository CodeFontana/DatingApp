namespace Client.Interfaces;

public interface ILikesService
{
    Task<ServiceResponseModel<IEnumerable<LikeUserModel>>> GetLikesAsync(string predicate);
    Task<ServiceResponseModel<string>> ToggleLikeAsync(string username);
}