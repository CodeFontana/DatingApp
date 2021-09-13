using System.Threading.Tasks;

namespace Client.Interfaces
{
    public interface IImageService
    {
        Task<string> RequestImageAsync(string imageUrl);
    }
}