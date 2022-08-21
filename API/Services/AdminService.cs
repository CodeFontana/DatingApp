﻿namespace API.Services;

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

    public async Task<PaginationResponseModel<PaginationList<UserWithRolesModel>>> GetUsersWithRolesAsync(string requestor, PaginationParameters pageParameters)
    {
        _logger.LogInformation($"Get users with roles... [{requestor}]");
        PaginationResponseModel<PaginationList<UserWithRolesModel>> pagedResponse = new();

        try
        {
            pagedResponse.Data = await _adminRepository.GetUsersWithRolesAsync(pageParameters);
            pagedResponse.Success = true;
            pagedResponse.MetaData = pagedResponse.Data.MetaData;
            pagedResponse.Message = $"Successfully listed User-Role relationships for [{requestor}]";
            _logger.LogInformation(pagedResponse.Message);
        }
        catch (Exception e)
        {
            pagedResponse.Success = false;
            pagedResponse.Message = $"Failed to get list of User-Role relationship for [{requestor}]";
            _logger.LogError(pagedResponse.Message);
            _logger.LogError(e.Message);
        }

        return pagedResponse;
    }

    public async Task<ServiceResponseModel<string>> EditRolesAsync(string requestor, UserWithRolesModel userWithRoles)
    {
        _logger.LogInformation($"Edit roles for {userWithRoles.Username}... [{requestor}]");
        ServiceResponseModel<string> serviceResponse = new();

        try
        {
            await _adminRepository.EditRolesAsync(userWithRoles);

            serviceResponse.Success = true;
            serviceResponse.Data = $"Successfully edited roles for user [{userWithRoles.Username}], requested by {requestor}";
            serviceResponse.Message = $"Successfully edited roles for user [{userWithRoles.Username}], requested by {requestor}";
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
