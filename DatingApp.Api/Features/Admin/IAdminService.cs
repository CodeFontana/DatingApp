using DatingApp.Contracts.Admin.Responses;
using DatingApp.Contracts.Common;

namespace DatingApp.Api.Features.Admin;

public interface IAdminService
{
    Task<ApiResponse<List<string>>> GetRolesAsync(string requestor);
    Task<ApiResponse<List<UserWithRolesResponse>>> GetUsersWithRolesAsync(string requestor);
    Task<ApiResponse<string>> EditRolesAsync(string requestor, UserWithRolesResponse userWithRoles);
    ApiResponse<string> GetPhotosForModeration(string requestor);
}
