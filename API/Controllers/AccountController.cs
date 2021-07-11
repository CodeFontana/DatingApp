using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthenticatedUser>> Register(AuthenticationUser userReg)
        {
            if (await UserExists(userReg.Username))
            {
                return BadRequest("Username is taken.");
            }

            AppUser user = new()
            {
                UserName = userReg.Username.ToLower(),
            };

            IdentityResult result = await _userManager.CreateAsync(user, userReg.Password);

            if (result.Succeeded == false)
            {
                return BadRequest(result.Errors);
            }

            return new AuthenticatedUser
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthenticatedUser>> Login(AuthenticationUser loginModel)
        {
            AppUser user = await _userManager.Users
                .SingleOrDefaultAsync(u => u.UserName == loginModel.Username.ToLower());

            if (user == null)
            {
                return Unauthorized("Invalid username.");
            }

            Microsoft.AspNetCore.Identity.SignInResult result = 
                await _signInManager.CheckPasswordSignInAsync(user, loginModel.Password, false);

            if (result.Succeeded == false)
            {
                return Unauthorized();
            }

            return new AuthenticatedUser
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(e => e.UserName == username.ToLower());
        }
    }
}
