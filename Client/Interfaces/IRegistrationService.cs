using DataAccessLibrary.Interfaces;
using System;
using System.Threading.Tasks;

namespace Client.Interfaces
{
    public interface IRegistrationService
    {
        Task<Tuple<bool, string>> RegisterAsync(IRegisterUser registerUser);
    }
}