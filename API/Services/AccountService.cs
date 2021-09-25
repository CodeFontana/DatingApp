using API.Interfaces;
using AutoMapper;
using DataAccessLibrary.Entities;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly IMapper _mapper;
        private readonly ILogger<AccountService> _logger;

        public AccountService(UserManager<AppUser> userManager,
                              SignInManager<AppUser> signInManager,
                              ITokenService tokenService,
                              IMapper mapper,
                              ILogger<AccountService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponseModel<AuthUserModel>> Register(RegisterUserModel registerUser)
        {
            ServiceResponseModel<AuthUserModel> serviceResponse = new();

            try
            {
                if (await UserExists(registerUser.Username))
                {
                    throw new ArgumentException($"Username is taken [{registerUser.Username}]");
                }

                AppUser user = _mapper.Map<AppUser>(registerUser);

                IdentityResult result = await _userManager.CreateAsync(user, registerUser.Password);

                if (result.Succeeded == false)
                {
                    throw new Exception($"Failed to register user [{user.UserName}]");
                }

                IdentityResult roleResult = await _userManager.AddToRoleAsync(user, "Member");

                if (roleResult.Succeeded == false)
                {
                    await _userManager.DeleteAsync(user);
                    throw new Exception($"Failed to register user [{user.UserName}]");
                }

                serviceResponse.Success = true;
                serviceResponse.Data = new AuthUserModel
                {
                    Username = user.UserName,
                    Token = await _tokenService.CreateTokenAsync(user)
                };
                serviceResponse.Message = $"Successfully registered user [{user.UserName}]";
                _logger.LogInformation(serviceResponse.Message);
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = e.Message;
                _logger.LogError(e.Message);
            }

            return serviceResponse;
        }

        public async Task<ServiceResponseModel<AuthUserModel>> Login(LoginUserModel loginUser)
        {
            ServiceResponseModel<AuthUserModel> serviceResponse = new();

            try
            {
                AppUser appUser = await _userManager.Users
                    .Include(p => p.Photos)
                    .SingleOrDefaultAsync(u => u.UserName == loginUser.Username.ToLower());

                if (appUser == null)
                {
                    throw new ArgumentException($"Invalid username [{loginUser.Username}]");
                }

                SignInResult result = await _signInManager.CheckPasswordSignInAsync(
                    appUser, loginUser.Password, false);

                if (result.Succeeded == false)
                {
                    throw new Exception($"Invalid password for user [{loginUser.Username}]");
                }

                serviceResponse.Success = true;
                serviceResponse.Data = new AuthUserModel
                {
                    Username = appUser.UserName,
                    Token = await _tokenService.CreateTokenAsync(appUser)
                };
                serviceResponse.Message = $"Successfully authenticated user [{appUser.UserName}]";
                _logger.LogInformation(serviceResponse.Message);
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = e.Message;
                _logger.LogError(e.Message);
            }

            return serviceResponse;
        }

        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(e => e.UserName == username.ToLower());
        }
    }
}
