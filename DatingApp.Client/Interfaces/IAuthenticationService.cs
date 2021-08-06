using DatingApp.Library.Interfaces;
using DatingApp.Library.Models;
using System.Threading.Tasks;

namespace DatingApp.Client.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthUserModel> LoginAsync(LoginUserModel loginUser);
        Task LogoutAsync();
    }
}