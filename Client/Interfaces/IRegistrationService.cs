using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;
using System;
using System.Threading.Tasks;

namespace Client.Interfaces
{
    public interface IRegistrationService
    {
        Task<ServiceResponseModel<AuthUserModel>> RegisterAsync(IRegisterUserModel registerUser);
    }
}