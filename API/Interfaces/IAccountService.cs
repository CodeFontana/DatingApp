using DataAccessLibrary.Models;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IAccountService
    {
        Task<ServiceResponseModel<AuthUserModel>> LoginAsync(LoginUserModel loginUser);
        Task<ServiceResponseModel<AuthUserModel>> RegisterAsync(RegisterUserModel registerUser);
    }
}