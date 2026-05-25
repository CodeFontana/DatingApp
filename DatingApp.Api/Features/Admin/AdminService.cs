using DatingApp.Contracts.Admin.Responses;
using DatingApp.Contracts.Common;
using DatingApp.DataAccess.Interfaces;
using DatingApp.DataAccess.Internal;

namespace DatingApp.Api.Features.Admin;

public class AdminService : IAdminService
{
    private readonly ILogger<AdminService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public AdminService(ILogger<AdminService> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<string>>> GetRolesAsync(string requestor)
    {
        _logger.LogInformation("Get roles... [{Requestor}]", requestor);
        ApiResponse<List<string>> response = new();

        try
        {
            response.Success = true;
            response.Data = await _unitOfWork.AdminRepository.GetRolesAsync();
            response.Message = $"Successfully listed User Roles [{requestor}]";
            _logger.LogInformation(response.Message);
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = $"Failed to get list of User Roles [{requestor}]";
            _logger.LogError(e, response.Message);
        }

        return response;
    }

    public async Task<ApiResponse<List<UserWithRolesResponse>>> GetUsersWithRolesAsync(string requestor)
    {
        _logger.LogInformation("Get users with roles... [{Requestor}]", requestor);
        ApiResponse<List<UserWithRolesResponse>> response = new();

        try
        {
            List<UserWithRolesReadModel> users = await _unitOfWork.AdminRepository.GetUsersWithRolesAsync();
            response.Success = true;
            response.Data = users.Select(AdminMapper.ToResponse).ToList();
            response.Message = $"Successfully listed User-Role relationships for [{requestor}]";
            _logger.LogInformation(response.Message);
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = $"Failed to get list of User-Role relationship for [{requestor}]";
            _logger.LogError(e, response.Message);
        }

        return response;
    }

    public async Task<ApiResponse<string>> EditRolesAsync(string requestor, UserWithRolesResponse userWithRoles)
    {
        _logger.LogInformation("Edit roles for {Username}... [{Requestor}]", userWithRoles.Username, requestor);
        ApiResponse<string> response = new();

        try
        {
            await _unitOfWork.AdminRepository.EditRolesAsync(AdminMapper.ToReadModel(userWithRoles));

            response.Success = true;
            response.Data = $"Successfully edited roles for user [{userWithRoles.Username}], requested by {requestor}";
            response.Message = response.Data;
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

    public ApiResponse<string> GetPhotosForModeration(string requestor)
    {
        _logger.LogInformation("Get photos for moderation... [{Requestor}]", requestor);
        return new ApiResponse<string>
        {
            Success = true,
            Data = "TODO: Admins or moderators can see this",
            Message = "TODO: Admins or moderators can see this"
        };
    }
}
