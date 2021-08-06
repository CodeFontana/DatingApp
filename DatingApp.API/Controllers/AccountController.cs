using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using DatingApp.API.Entities;
using DatingApp.API.Interfaces;
using DatingApp.Library.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatingApp.Library.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace DatingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountController : ControllerBase
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
        [AllowAnonymous]
        public async Task<ActionResult<AuthUserModel>> Register(RegisterUserModel registerUser)
        {
            if (await UserExists(registerUser.Username))
            {
                return BadRequest("Username is taken.");
            }

            AppUser user = new()
            {
                UserName = registerUser.Username.ToLower()
            };

            IdentityResult result = await _userManager.CreateAsync(user, registerUser.Password);

            if (result.Succeeded == false)
            {
                return BadRequest(result.Errors);
            }

            IdentityResult roleResult = await _userManager.AddToRoleAsync(user, "Member");

            if (roleResult.Succeeded == false)
            {
                await _userManager.DeleteAsync(user);
                return BadRequest(result.Errors);
            }

            return new AuthUserModel
            {
                Username = user.UserName,
                Token = await _tokenService.CreateTokenAsync(user)
            };
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthUserModel>> Login(LoginUserModel loginUser)
        {
            AppUser user = await _userManager.Users
                .SingleOrDefaultAsync(u => u.UserName == loginUser.Username.ToLower());

            if (user == null)
            {
                return Unauthorized("Invalid username.");
            }

            Microsoft.AspNetCore.Identity.SignInResult result = 
                await _signInManager.CheckPasswordSignInAsync(user, loginUser.Password, false);

            if (result.Succeeded == false)
            {
                return Unauthorized();
            }

            return new AuthUserModel
            {
                Username = user.UserName,
                Token = await _tokenService.CreateTokenAsync(user)
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(e => e.UserName == username.ToLower());
        }
    }
}
