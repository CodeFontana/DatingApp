using DatingApp.Contracts.Authentication.Requests;
using DatingApp.Contracts.Authentication.Responses;
using DatingApp.DataAccess.Entities;
using DatingApp.DataAccess.Internal;

namespace DatingApp.Api.Features.Authentication;

internal static class AuthenticationMapper
{
    public static RegisterAccountData ToRegisterData(RegisterRequest request) => new()
    {
        Username = request.Username,
        Email = request.Email,
        Password = request.Password
    };

    public static LoginCredentials ToLoginCredentials(LoginRequest request) => new()
    {
        Username = request.Username,
        Password = request.Password
    };

    public static AccountUpdateData ToUpdateData(AccountUpdateRequest request) => new()
    {
        Id = request.Id,
        UserName = request.UserName,
        Email = request.Email
    };

    public static AuthResponse ToAuthResponse(AppUser user, string token) => new()
    {
        Username = user.UserName ?? string.Empty,
        Token = token
    };

    public static AccountResponse ToAccountResponse(AppUser user) => new()
    {
        Id = user.Id,
        Username = user.UserName ?? string.Empty,
        Email = user.Email ?? string.Empty,
        Created = user.Created,
        LastActive = user.LastActive
    };
}
