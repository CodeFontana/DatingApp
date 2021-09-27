using DataAccessLibrary.Models;
using DataAccessLibrary.Paging;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client.Interfaces
{
    public interface IMemberService
    {
        Task<ServiceResponseModel<PhotoModel>> AddPhotoAsync(string username, MultipartFormDataContent content);
        Task<ServiceResponseModel<string>> DeletePhotoAsync(string username, PhotoModel photo);
        Task<ServiceResponseModel<MemberModel>> GetMemberAsync(string username);
        Task<PagingResponseModel<IEnumerable<MemberModel>>> GetMembersAsync(UserParameters userParameters);
        Task<string> GetPhotoAsync(string username, string filename);
        Task<ServiceResponseModel<string>> SetMainPhotoAsync(string username, int photoId);
        Task<ServiceResponseModel<string>> UpdateMemberAsync(MemberUpdateModel memberUpdate);
    }
}