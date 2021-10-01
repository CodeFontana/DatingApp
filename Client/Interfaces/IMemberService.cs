using DataAccessLibrary.Models;
using DataAccessLibrary.Pagination;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client.Interfaces
{
    public interface IMemberService
    {
        List<MemberModel> MemberCache { get; set; }
        Dictionary<string, MemberCacheModel> MemberListCache { get; set; }

        Task<ServiceResponseModel<PhotoModel>> AddPhotoAsync(string username, MultipartFormDataContent content);
        Task<ServiceResponseModel<string>> DeletePhotoAsync(string username, PhotoModel photo);
        Task<ServiceResponseModel<MemberModel>> GetMemberAsync(string username);
        Task<PaginationResponseModel<IEnumerable<MemberModel>>> GetMembersAsync(UserParameters userParameters);
        Task<string> GetPhotoAsync(string username, string filename);
        Task<ServiceResponseModel<string>> SetMainPhotoAsync(string username, int photoId);
        Task<ServiceResponseModel<string>> UpdateMemberAsync(MemberUpdateModel memberUpdate);
    }
}