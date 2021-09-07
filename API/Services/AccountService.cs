using API.Interfaces;
using DataAccessLibrary.Entities;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AccountService(UserManager<AppUser> userManager,
                              SignInManager<AppUser> signInManager,
                              ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<ServiceResponseModel<AuthUserModel>> Register(RegisterUserModel registerUser)
        {
            ServiceResponseModel<AuthUserModel> serviceResponse = new();

            try
            {
                if (await UserExists(registerUser.Username))
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"Username is taken [{registerUser.Username}]";
                    Console.WriteLine($"Username is taken [{registerUser.Username}]");
                    return serviceResponse;
                }

                AppUser user = new()
                {
                    UserName = registerUser.Username
                };

                IdentityResult result = await _userManager.CreateAsync(user, registerUser.Password);

                if (result.Succeeded == false)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"Failed to register user [{user.UserName}]";
                    Console.WriteLine(result.Errors);
                    Console.WriteLine($"Failed to create user [{user.UserName}]");
                    return serviceResponse;
                }

                IdentityResult roleResult = await _userManager.AddToRoleAsync(user, "Member");

                if (roleResult.Succeeded == false)
                {
                    await _userManager.DeleteAsync(user);
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"Failed to register user [{user.UserName}]";
                    Console.WriteLine(result.Errors);
                    Console.WriteLine($"Failed to add user [{user.UserName}] to role [Member]");
                    return serviceResponse;
                }

                serviceResponse.Success = true;
                serviceResponse.Data = new AuthUserModel
                {
                    Username = user.UserName,
                    Token = await _tokenService.CreateTokenAsync(user)
                };
                serviceResponse.Message = $"Successfully registered user [{user.UserName}]";
                Console.WriteLine($"Successfully registered user [{user.UserName}]");
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Failed to register user [{registerUser.Username}]";
                Console.WriteLine(e.Message);
                Console.WriteLine($"Failed to register user [{registerUser.Username}]");
            }

            return serviceResponse;
        }

        public async Task<ServiceResponseModel<AuthUserModel>> Login(LoginUserModel loginUser)
        {
            ServiceResponseModel<AuthUserModel> serviceResponse = new();

            try
            {
                AppUser user = await _userManager.Users.SingleOrDefaultAsync(
                    u => u.UserName == loginUser.Username.ToLower());

                if (user == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"Invalid username [{loginUser.Username}]";
                    Console.WriteLine($"Invalid username [{loginUser.Username}]");
                    return serviceResponse;
                }

                SignInResult result = await _signInManager.CheckPasswordSignInAsync(
                    user, loginUser.Password, false);

                if (result.Succeeded == false)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Invalid password";
                    Console.WriteLine("Invalid password");
                    return serviceResponse;
                }

                serviceResponse.Success = true;
                serviceResponse.Data = new AuthUserModel
                {
                    Username = user.UserName,
                    Token = await _tokenService.CreateTokenAsync(user)
                };
                serviceResponse.Message = $"Successfully authenticated user [{user.UserName}]";
                Console.WriteLine($"Successfully authenticated user [{user.UserName}]");
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = e.Message;
                Console.WriteLine(e.Message);
                Console.WriteLine($"Failed to login user [{loginUser.Username}]");
            }

            return serviceResponse;
        }

        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(e => e.UserName == username.ToLower());
        }
    }
}
