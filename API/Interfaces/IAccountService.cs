using DataAccessLibrary.Models;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IAccountService
    {
        Task<ServiceResponseModel<AuthUserModel>> Login(LoginUserModel loginUser);
        Task<ServiceResponseModel<AuthUserModel>> Register(RegisterUserModel registerUser);
    }
}