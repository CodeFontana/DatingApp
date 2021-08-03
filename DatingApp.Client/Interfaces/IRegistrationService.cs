using DatingApp.Library.Interfaces;
using System.Threading.Tasks;

namespace DatingApp.Client.Interfaces
{
    public interface IRegistrationService
    {
        Task<IAuthUserModel> RegisterAsync(IRegisterUserModel registerUser);
    }
}