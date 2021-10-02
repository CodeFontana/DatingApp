using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUserLikeService
    {
        Task<ServiceResponseModel<string>> AddLikeAsync(string reqeustor, string username, int sourceUserId);
        Task<ServiceResponseModel<IEnumerable<LikeUserModel>>> GetUserLikesAsync(string requestor, string predicate, int sourceUserId);
    }
}