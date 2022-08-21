namespace API.Interfaces;

public interface IAdminService
{
    Task<ServiceResponseModel<List<UserWithRolesModel>>> GetUsersWithRolesAsync(string requestor);
    Task<ServiceResponseModel<string>> EditRolesAsync(string requestor, UserWithRolesModel userWithRoles);
    ServiceResponseModel<string> GetPhotosForModeration(string requestor);
}