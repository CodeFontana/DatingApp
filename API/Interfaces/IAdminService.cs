using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Interfaces;

public interface IAdminService
{
    Task<ServiceResponseModel<List<UserWithRolesModel>>> GetUsersWithRolesAsync(string requestor);
    Task<ServiceResponseModel<string>> EditRolesAsync(string requestor, UserWithRolesModel userWithRoles);
    ServiceResponseModel<string> GetPhotosForModeration(string requestor);
    Task<ServiceResponseModel<List<string>>> GetRolesAsync(string requestor);
}