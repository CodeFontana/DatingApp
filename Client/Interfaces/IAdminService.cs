namespace Client.Interfaces;

public interface IAdminService
{
    Task<ServiceResponseModel<string>> EditRolesAsync(UserWithRolesModel userWithRoles);
    Task<ServiceResponseModel<IEnumerable<UserWithRolesModel>>> GetUsersWithRolesAsync();
}