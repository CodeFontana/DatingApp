using DatingApp.Contracts.Authentication.Requests;
using DatingApp.Contracts.Authentication.Responses;
using DatingApp.Contracts.Common;
using DatingApp.DataAccess.Entities;
using DatingApp.DataAccess.Interfaces;
using DatingApp.Api.Infrastructure.Authentication;
using Microsoft.AspNetCore.Identity;

namespace DatingApp.Api.Features.Authentication;

public sealed class AccountService : IAccountService
{
    private readonly ILogger<AccountService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;

    public AccountService(ILogger<AccountService> logger, IUnitOfWork unitOfWork, ITokenService tokenService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
    }

    public async Task<ApiResponse<AuthResponse>> RegisterAsync(string? requestor, RegisterRequest registerUser)
    {
        _logger.LogInformation("Register new user {Email}... [{Requestor}]", registerUser.Email, requestor);
        ApiResponse<AuthResponse> response = new();

        try
        {
            AppUser appUser = await _unitOfWork.AccountRepository.CreateAccountAsync(
                AuthenticationMapper.ToRegisterData(registerUser));

            response.Success = true;
            response.Data = AuthenticationMapper.ToAuthResponse(
                appUser,
                await _tokenService.CreateTokenAsync(appUser));
            response.Message = $"Successfully registered user [{appUser.UserName}]";
            _logger.LogInformation(response.Message);
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
            _logger.LogError(e.Message);
        }

        return response;
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest loginUser)
    {
        ApiResponse<AuthResponse> response = new();

        try
        {
            AppUser appUser = await _unitOfWork.AccountRepository.LoginAsync(
                AuthenticationMapper.ToLoginCredentials(loginUser));

            response.Success = true;
            response.Data = AuthenticationMapper.ToAuthResponse(
                appUser,
                await _tokenService.CreateTokenAsync(appUser));
            response.Message = $"Successfully authenticated user [{appUser.UserName}]";
            _logger.LogInformation(response.Message);
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
            _logger.LogError(e.Message);
        }

        return response;
    }

    public async Task<ApiResponse<AccountResponse>> GetAccountAsync(string requestor, string username)
    {
        _logger.LogInformation("Get user account {Username}... [{Requestor}]", username, requestor);
        ApiResponse<AccountResponse> response = new();

        try
        {
            AppUser? appUser = await _unitOfWork.AccountRepository.GetAccountAsync(username)
                ?? throw new Exception("Username not found");

            response.Success = true;
            response.Data = AuthenticationMapper.ToAccountResponse(appUser);
            response.Message = $"Successfully retrieved user [{response.Data.Username}]";
            _logger.LogInformation(response.Message);
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
            _logger.LogError(e.Message);
        }

        return response;
    }

    public async Task<ApiResponse<List<AccountResponse>>> GetAccountsAsync(string requestor)
    {
        _logger.LogInformation("Get user accounts... [{Requestor}]", requestor);
        ApiResponse<List<AccountResponse>> response = new();

        try
        {
            List<AppUser> appUsers = await _unitOfWork.AccountRepository.GetAccountsAsync();
            List<AccountResponse> accounts = appUsers.Select(AuthenticationMapper.ToAccountResponse).ToList();

            response.Success = true;
            response.Data = accounts;
            response.Message = $"Successfully retrieved users [Count={accounts.Count}]";
            _logger.LogInformation(response.Message);
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
            _logger.LogError(e.Message);
        }

        return response;
    }

    public async Task<ApiResponse<bool>> UpdateAccountAsync(string requestor, AccountUpdateRequest updateAccount)
    {
        _logger.LogInformation("Update user account {Id}/{UserName}... [{Requestor}]", updateAccount.Id, updateAccount.UserName, requestor);
        ApiResponse<bool> response = new();

        try
        {
            await _unitOfWork.AccountRepository.UpdateAccountAsync(
                AuthenticationMapper.ToUpdateData(updateAccount));

            if (_unitOfWork.HasChanges())
            {
                await _unitOfWork.CompleteAsync();
            }

            response.Success = true;
            response.Data = true;
            response.Message = $"Successfully updated user [{updateAccount.UserName}]";
            _logger.LogInformation(response.Message);
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
            _logger.LogError(e.Message);
        }

        return response;
    }

    public async Task<ApiResponse<bool>> DeleteAccountAsync(string requestor, string username)
    {
        _logger.LogInformation("Delete user account {Username}... [{Requestor}]", username, requestor);
        ApiResponse<bool> response = new();

        try
        {
            IdentityResult result = await _unitOfWork.AccountRepository.DeleteAccountAsync(requestor, username);

            if (result.Succeeded)
            {
                response.Success = true;
                response.Data = true;
                response.Message = $"Successfully deleted user [{username}] -- {result}";
                _logger.LogInformation(response.Message);
            }
            else
            {
                throw new Exception($"Failed to delete user [{username}] -- {result}");
            }
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
            _logger.LogError(e.Message);
        }

        return response;
    }
}
