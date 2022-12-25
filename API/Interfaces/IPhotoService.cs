using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Interfaces;

public interface IPhotoService
{
    Task<ServiceResponseModel<PhotoModel>> AddPhotoAsync(string username, IEnumerable<IFormFile> file);
    Task<ServiceResponseModel<string>> DeletePhotoAsync(string username, PhotoModel photo);
    Task<ServiceResponseModel<byte[]>> GetPhotoAsync(string username, string filename);
    Task<ServiceResponseModel<string>> SetMainPhotoAsync(string username, int photoId);
}
