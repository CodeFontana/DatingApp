using DataAccessLibrary.Models;
using System;
using System.Threading.Tasks;

namespace Client.Interfaces
{
    public interface IAppUserService
    {
        MemberModel AppUser { get; }
        string MainPhoto { get; }

        event Action OnChange;

        Task<bool> ReloadAppUser();
        Task<bool> SetAppUser(string username);
        Task SetMainPhoto(string filename);
    }
}