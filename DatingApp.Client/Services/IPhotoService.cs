namespace DatingApp.Client.Services;

public interface IPhotoService
{
    Task<ApiResponse<PhotoResponse>> AddPhotoAsync(string username, MultipartFormDataContent content);
    Task<ApiResponse<string>> DeletePhotoAsync(string username, PhotoResponse photo);
    Task<string> GetPhotoAsync(string username, string filename);
    Task<ApiResponse<string>> SetMainPhotoAsync(string username, int photoId);
}