namespace DatingApp.Client.Services;

public interface IAdminService
{
    Task<ApiResponse<bool>> DeleteAccountAsync(string username);
    Task<ApiResponse<string>> EditRolesAsync(UserWithRolesResponse userWithRoles);
    Task<ApiResponse<IEnumerable<string>>> GetRolesAsync();
    Task<ApiResponse<IEnumerable<UserWithRolesResponse>>> GetUsersWithRolesAsync();
}