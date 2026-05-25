using DatingApp.DataAccess.Entities;

namespace DatingApp.Api.Infrastructure.Authentication;

public interface ITokenService
{
    Task<string> CreateTokenAsync(AppUser user);
}
