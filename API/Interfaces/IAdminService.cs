using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IAdminService
    {
        Task<ServiceResponseModel<IList<string>>> EditRoles(string username, string roles);
        ServiceResponseModel<string> GetPhotosForModeration();
        Task<ServiceResponseModel<List<UserWithRolesModel>>> GetUsersWithRoles(string requestor);
    }
}