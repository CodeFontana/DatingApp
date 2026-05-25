using DatingApp.Contracts.Common;
using DatingApp.Contracts.Members.Responses;

namespace DatingApp.Api.Features.Members;

public interface IPhotoService
{
    Task<ApiResponse<PhotoResponse>> AddPhotoAsync(string username, IEnumerable<IFormFile> files);
    Task<ApiResponse<byte[]>> GetPhotoAsync(string username, string filename);
    Task<ApiResponse<string>> SetMainPhotoAsync(string username, int photoId);
    Task<ApiResponse<string>> DeletePhotoAsync(string username, PhotoResponse photo);
}
