namespace API.Services;

public class AdminService : IAdminService
{
    private readonly ILogger<AdminService> _logger;
    private readonly IAdminRepository _adminRepository;

    public AdminService(ILogger<AdminService> logger,
                        IAdminRepository adminRepository)
    {
        _logger = logger;
        _adminRepository = adminRepository;
    }

    public async Task<ServiceResponseModel<List<UserWithRolesModel>>> GetUsersWithRolesAsync(string requestor)
    {
        _logger.LogInformation($"Get users with roles... [{requestor}]");
        ServiceResponseModel<List<UserWithRolesModel>> serviceResponse = new();

        try
        {
            serviceResponse.Data = await _adminRepository.GetUsersWithRolesAsync();
            serviceResponse.Success = true;
            serviceResponse.Message = $"Successfully listed User-Role relationships for [{requestor}]";
            _logger.LogInformation(serviceResponse.Message);
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = $"Failed to get list of User-Role relationship for [{requestor}]";
            _logger.LogError(serviceResponse.Message);
            _logger.LogError(e.Message);
        }

        return serviceResponse;
    }

    public async Task<ServiceResponseModel<IList<string>>> EditRolesAsync(string requestor, string username, string roles)
    {
        _logger.LogInformation($"Edit roles for {username}... [{requestor}]");
        ServiceResponseModel<IList<string>> serviceResponse = new();

        try
        {
            serviceResponse.Data = await _adminRepository.EditRolesAsync(username, roles);
            serviceResponse.Success = true;
            serviceResponse.Message = $"Successfully editted roles for user [{username}], requested by {requestor}";
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

    public ServiceResponseModel<string> GetPhotosForModeration(string requestor)
    {
        _logger.LogInformation($"Get photos for moderation... [{requestor}]");
        ServiceResponseModel<string> serviceResponse = new();

        try
        {
            serviceResponse.Success = true;
            serviceResponse.Data = "TODO: Admins or moderators can see this";
            serviceResponse.Message = "TODO: Admins or moderators can see this";
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
}
