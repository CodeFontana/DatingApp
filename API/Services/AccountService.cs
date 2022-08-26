namespace API.Services;

public class AccountService : IAccountService
{
    private readonly ILogger<AccountService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;

    public AccountService(ILogger<AccountService> logger,
                          IUnitOfWork unitOfWork,
                          ITokenService tokenService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
    }

    public async Task<ServiceResponseModel<AuthUserModel>> RegisterAsync(string requestor, RegisterUserModel registerUser)
    {
        _logger.LogInformation($"Register new user {registerUser.Email}... [{requestor}]");
        ServiceResponseModel<AuthUserModel> serviceResponse = new();

        try
        {
            AppUser appUser = await _unitOfWork.AccountRepository.CreateAccountAsync(registerUser);

            serviceResponse.Success = true;
            serviceResponse.Data = new AuthUserModel
            {
                Username = appUser.UserName,
                Token = await _tokenService.CreateTokenAsync(appUser)
            };
            serviceResponse.Message = $"Successfully registered user [{appUser.UserName}]";
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

    public async Task<ServiceResponseModel<AuthUserModel>> LoginAsync(LoginUserModel loginUser)
    {
        ServiceResponseModel<AuthUserModel> serviceResponse = new();

        try
        {
            AppUser appUser = await _unitOfWork.AccountRepository.LoginAsync(loginUser);

            serviceResponse.Success = true;
            serviceResponse.Data = new AuthUserModel
            {
                Username = appUser.Email,
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

    public async Task<ServiceResponseModel<AccountModel>> GetAccountAsync(string requestor, string username)
    {
        _logger.LogInformation($"Get user account {username}... [{requestor}]");
        ServiceResponseModel<AccountModel> serviceResponse = new();

        try
        {
            AppUser appUser = await _unitOfWork.AccountRepository.GetAccountAsync(username);

            if (appUser != null)
            {
                AccountModel appAcount = new()
                {
                    Id = appUser.Id,
                    Username = appUser.UserName,
                    Email = appUser.Email,
                    Created = appUser.Created,
                    LastActive = appUser.LastActive
                };

                serviceResponse.Success = true;
                serviceResponse.Data = appAcount;
                serviceResponse.Message = $"Successfully retrieved user [{appAcount.Username}]";
                _logger.LogInformation(serviceResponse.Message);
            }
            else
            {
                throw new Exception("Username not found");
            }
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
            _logger.LogError(e.Message);
        }

        return serviceResponse;
    }

    public async Task<ServiceResponseModel<List<AccountModel>>> GetAccountsAsync(string requestor)
    {
        _logger.LogInformation($"Get user accounts... [{requestor}]");
        ServiceResponseModel<List<AccountModel>> serviceResponse = new();

        try
        {
            List<AppUser> appUsers = await _unitOfWork.AccountRepository.GetAccountsAsync();

            if (appUsers != null)
            {
                List<AccountModel> appAcount = new();

                foreach (AppUser appUser in appUsers)
                {
                    appAcount.Add(new AccountModel()
                    {
                        Id = appUser.Id,
                        Username = appUser.UserName,
                        Email = appUser.Email,
                        Created = appUser.Created,
                        LastActive = appUser.LastActive
                    });
                }

                serviceResponse.Success = true;
                serviceResponse.Data = appAcount;
                serviceResponse.Message = $"Successfully retrieved users [Count={appAcount.Count}]";
                _logger.LogInformation(serviceResponse.Message);
            }
            else
            {
                throw new Exception("No users found");
            }
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
            _logger.LogError(e.Message);
        }

        return serviceResponse;
    }

    public async Task<ServiceResponseModel<bool>> UpdateAccountAsync(string requestor, AccountUpdateModel updateAccount)
    {
        _logger.LogInformation($"Update user account {updateAccount.Id}/{updateAccount.UserName}... [{requestor}]");
        ServiceResponseModel<bool> serviceResponse = new();

        try
        {
            await _unitOfWork.AccountRepository.UpdateAccountAsync(updateAccount);

            if (_unitOfWork.HasChanges())
            {
                await _unitOfWork.CompleteAsync();
            }

            serviceResponse.Success = true;
            serviceResponse.Data = true;
            serviceResponse.Message = $"Successfully updated user [{updateAccount.UserName}]";
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

    public async Task<ServiceResponseModel<bool>> DeleteAccountAsync(string requestor, string username)
    {
        _logger.LogInformation($"Delete user account {username}... [{requestor}]");
        ServiceResponseModel<bool> serviceResponse = new();

        try
        {
            IdentityResult result = await _unitOfWork.AccountRepository.DeleteAccountAsync(requestor, username);

            if (result.Succeeded)
            {
                serviceResponse.Success = result.Succeeded;
                serviceResponse.Data = result.Succeeded;
                serviceResponse.Message = $"Successfully deleted user [{username}] -- {result}";
                _logger.LogInformation(serviceResponse.Message);
            }
            else
            {
                throw new Exception($"Failed to delete user [{username}] -- {result}");
            }
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
            _logger.LogError(e.Message);
        }

        return serviceResponse;
    }
}
