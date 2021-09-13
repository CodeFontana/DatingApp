using DataAccessLibrary.Models;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IImagesService
    {
        Task<ServiceResponseModel<byte[]>> GetImage(string username, string filename);
    }
}