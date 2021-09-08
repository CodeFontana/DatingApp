using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUsersService
    {
        Task<ServiceResponseModel<MemberModel>> GetUser(string username, string requestor);
        Task<ServiceResponseModel<IEnumerable<MemberModel>>> GetUsers(string requestor);
        Task<ServiceResponseModel<string>> UpdateUser(string username, MemberUpdateModel memberUpdate);
    }
}