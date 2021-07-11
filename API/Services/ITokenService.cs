using API.Entities;
using System.Threading.Tasks;

namespace API.Services
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}