using DataAccessLibrary.Models;
using DataAccessLibrary.Pagination;
using System.Threading.Tasks;

namespace API.Interfaces;

public interface ILikesService
{
    Task<PaginationResponseModel<PaginationList<MemberModel>>> GetUserLikesAsync(string requestor, LikesParameters likesParameters);
    Task<ServiceResponseModel<string>> ToggleLikeAsync(string reqeustor, string username, int sourceUserId);
}