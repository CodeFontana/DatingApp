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

        Task<bool> ReloadAppUserAsync();
        Task<bool> SetAppUserAsync(string username);
        Task SetMainPhotoAsync(string filename);
    }
}