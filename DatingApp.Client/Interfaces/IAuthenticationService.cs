using DatingApp.Library.Models;
using System.Threading.Tasks;

namespace DatingApp.Client.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthUserModel> Login(LoginUserModel loginUser);
        Task Logout();
    }
}