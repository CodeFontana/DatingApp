using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;
using System;
using System.Threading.Tasks;

namespace Client.Interfaces
{
    public interface IAuthenticationService
    {
        Task<Tuple<AuthUser, string>> LoginAsync(LoginUser loginUser);
        Task LogoutAsync();
    }
}