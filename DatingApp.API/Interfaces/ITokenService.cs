using DatingApp.API.Entities;
using System.Threading.Tasks;

namespace DatingApp.API.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}