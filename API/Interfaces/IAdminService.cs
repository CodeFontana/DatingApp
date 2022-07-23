namespace API.Interfaces;

public interface IAdminService
{
    Task<ServiceResponseModel<IList<string>>> EditRolesAsync(string username, string roles);
    ServiceResponseModel<string> GetPhotosForModeration();
    Task<ServiceResponseModel<List<UserWithRolesModel>>> GetUsersWithRolesAsync(string requestor);
}