namespace Client.Interfaces;

public interface IAdminService
{
    Task<ServiceResponseModel<bool>> DeleteAccountAsync(string username);
    Task<ServiceResponseModel<string>> EditRolesAsync(UserWithRolesModel userWithRoles);
    Task<ServiceResponseModel<IEnumerable<string>>> GetRolesAsync();
    Task<ServiceResponseModel<IEnumerable<UserWithRolesModel>>> GetUsersWithRolesAsync();
}