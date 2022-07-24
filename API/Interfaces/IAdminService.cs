namespace API.Interfaces;

public interface IAdminService
{
    Task<ServiceResponseModel<IList<string>>> EditRolesAsync(string requestor, string username, string roles);
    ServiceResponseModel<string> GetPhotosForModeration(string requestor);
    Task<ServiceResponseModel<List<UserWithRolesModel>>> GetUsersWithRolesAsync(string requestor);
}