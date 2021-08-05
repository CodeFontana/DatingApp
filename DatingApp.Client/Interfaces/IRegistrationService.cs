using DatingApp.Library.Interfaces;
using System;
using System.Threading.Tasks;

namespace DatingApp.Client.Interfaces
{
    public interface IRegistrationService
    {
        Task<Tuple<bool, string>> RegisterAsync(IRegisterUserModel registerUser);
    }
}