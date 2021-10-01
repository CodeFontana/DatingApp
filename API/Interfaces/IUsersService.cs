using DataAccessLibrary.Models;
using DataAccessLibrary.Pagination;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUsersService
    {
        Task<ServiceResponseModel<MemberModel>> GetUserAsync(string username, string requestor);
        Task<PaginationResponseModel<PaginationList<MemberModel>>> GetUsersAsync(string requestor, UserParameters userParameters);
        Task<ServiceResponseModel<string>> UpdateUserAsync(string username, MemberUpdateModel memberUpdate);
    }
}