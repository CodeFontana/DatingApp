using DataAccessLibrary.Models;
using System;
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
        Task<ServiceResponseModel<IEnumerable<MemberModel>>> GetMembersAsync();
        Task<string> GetPhotoAsync(string username, string filename);
        Task<ServiceResponseModel<string>> SetMainPhotoAsync(string username, int photoId);
        Task<ServiceResponseModel<string>> UpdateMemberAsync(MemberUpdateModel memberUpdate);
    }
}