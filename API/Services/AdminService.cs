using API.Interfaces;
using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services;

public class AdminService : IAdminService
{
    private readonly ILogger<AdminService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public AdminService(ILogger<AdminService> logger,
                        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResponseModel<List<string>>> GetRolesAsync(string requestor)
    {
        _logger.LogInformation($"Get roles... [{requestor}]");
        ServiceResponseModel<List<string>> serviceResponse = new();

        try
        {
            serviceResponse.Success = true;
            serviceResponse.Data = await _unitOfWork.AdminRepository.GetRolesAsync();
            serviceResponse.Message = $"Successfully listed User Roles [{requestor}]";
            _logger.LogInformation(serviceResponse.Message);
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = $"Failed to get list of User Roles [{requestor}]";
            _logger.LogError(serviceResponse.Message);
            _logger.LogError(e.Message);
        }

        return serviceResponse;
    }

    public async Task<ServiceResponseModel<List<UserWithRolesModel>>> GetUsersWithRolesAsync(string requestor)
    {
        _logger.LogInformation($"Get users with roles... [{requestor}]");
        ServiceResponseModel<List<UserWithRolesModel>> serviceResponse = new();

        try
        {
            serviceResponse.Success = true;
            serviceResponse.Data = await _unitOfWork.AdminRepository.GetUsersWithRolesAsync();
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

    public async Task<ServiceResponseModel<string>> EditRolesAsync(string requestor, UserWithRolesModel userWithRoles)
    {
        _logger.LogInformation($"Edit roles for {userWithRoles.Username}... [{requestor}]");
        ServiceResponseModel<string> serviceResponse = new();

        try
        {
            await _unitOfWork.AdminRepository.EditRolesAsync(userWithRoles);

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
