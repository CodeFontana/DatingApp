using DatingApp.Library.Interfaces;
using DatingApp.Library.Models;
using System;
using System.Threading.Tasks;

namespace DatingApp.Client.Interfaces
{
    public interface IAuthenticationService
    {
        Task<Tuple<AuthUserModel, string>> LoginAsync(LoginUserModel loginUser);
        Task LogoutAsync();
    }
}