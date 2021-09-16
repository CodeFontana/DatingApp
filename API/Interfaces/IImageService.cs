using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IImageService
    {
        Task<ServiceResponseModel<byte[]>> GetImage(string username, string filename);
        Task<ServiceResponseModel<List<byte[]>>> GetImages(string username);
    }
}