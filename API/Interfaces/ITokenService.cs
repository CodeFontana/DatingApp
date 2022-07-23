namespace API.Interfaces;

public interface ITokenService
{
    Task<string> CreateTokenAsync(AppUser user);
}