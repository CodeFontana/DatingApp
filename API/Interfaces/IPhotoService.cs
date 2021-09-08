using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IPhotoService
    {
        Task<ServiceResponseModel<PhotoModel>> AddPhotoAsync(string username, IFormFile file);
        Task<ServiceResponseModel<string>> DeletePhotoAsync(string username, int photoId);
    }
}