using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DatingApp.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.Api.Infrastructure.Authentication;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _key;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private readonly int _jwtLifetimeMinutes;
    private readonly UserManager<AppUser> _userManager;

    public TokenService(IConfiguration config, UserManager<AppUser> userManager)
    {
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Authentication:JwtSecurityKey"]!));
        _jwtIssuer = config["Authentication:JwtIssuer"]!;
        _jwtAudience = config["Authentication:JwtAudience"]!;
        _jwtLifetimeMinutes = int.Parse(config["Authentication:JwtExpiryInMinutes"]!);
        _userManager = userManager;
    }

    public async Task<string> CreateTokenAsync(AppUser appUser)
    {
        List<Claim> claims =
        [
            new Claim(ClaimTypes.Name, appUser.UserName ?? string.Empty),
            new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, appUser.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, appUser.UserName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Iss, _jwtIssuer),
            new Claim(JwtRegisteredClaimNames.Aud, _jwtAudience),
            new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()),
            new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.UtcNow.AddMinutes(_jwtLifetimeMinutes)).ToUnixTimeSeconds().ToString())
        ];

        claims.AddRange(await _userManager.GetClaimsAsync(appUser));

        IList<string> roles = await _userManager.GetRolesAsync(appUser);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        SigningCredentials signingCredentials = new(_key, SecurityAlgorithms.HmacSha256);
        JwtSecurityToken token = new(new JwtHeader(signingCredentials), new JwtPayload(claims));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
