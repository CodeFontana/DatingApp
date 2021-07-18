using Client.Models;
using System.Threading.Tasks;

namespace Client.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthenticatedUserModel> Login(AuthenticationUserModel loginUser);
        Task Logout();
    }
}