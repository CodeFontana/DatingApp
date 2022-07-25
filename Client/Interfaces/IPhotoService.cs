namespace Client.Interfaces;

public interface IPhotoService
{
    Task<ServiceResponseModel<PhotoModel>> AddPhotoAsync(string username, MultipartFormDataContent content);
    Task<ServiceResponseModel<string>> DeletePhotoAsync(string username, PhotoModel photo);
    Task<string> GetPhotoAsync(string username, string filename);
    Task<ServiceResponseModel<string>> SetMainPhotoAsync(string username, int photoId);
}