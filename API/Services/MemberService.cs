using System;
using System.Threading.Tasks;
using API.Interfaces;
using DataAccessLibrary.Entities;
using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;
using DataAccessLibrary.Pagination;
using Microsoft.Extensions.Logging;

namespace API.Services;

public sealed class MemberService : IMemberService
{
    private readonly ILogger<MemberService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public MemberService(ILogger<MemberService> logger,
                         IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResponseModel<MemberModel>> GetMemberAsync(string username, string requestor)
    {
        _logger.LogInformation($"Get member {username}... [{requestor}]");
        ServiceResponseModel<MemberModel> serviceResponse = new();

        try
        {
            serviceResponse.Success = true;
            serviceResponse.Data = await _unitOfWork.MemberRepository.GetMemberAsync(username);
            serviceResponse.Message = $"Successfully retrieved [{username}] for [{requestor}]";
            _logger.LogInformation(serviceResponse.Message);
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = $"Failed to retrieve [{username}] for [{requestor}]";
            _logger.LogError(serviceResponse.Message);
            _logger.LogError(e.Message);
        }

        return serviceResponse;
    }

    public async Task<PaginationResponseModel<PaginationList<MemberModel>>> GetMembersAsync(string requestor, MemberParameters userParameters)
    {
        _logger.LogInformation($"Get members... [{requestor}]");
        PaginationResponseModel<PaginationList<MemberModel>> pagedResponse = new();

        try
        {
            userParameters.CurrentUsername = requestor;
            PaginationList<MemberModel> data = await _unitOfWork.MemberRepository.GetMembersAsync(userParameters);

            pagedResponse.Success = true;
            pagedResponse.Data = data;
            pagedResponse.MetaData = data.MetaData;
            pagedResponse.Message = $"Successfully listed users for [{requestor}]";
            _logger.LogInformation(pagedResponse.Message);
        }
        catch (Exception e)
        {
            pagedResponse.Success = false;
            pagedResponse.Message = $"Failed to list users for [{requestor}]";
            _logger.LogError(pagedResponse.Message);
            _logger.LogError(e.Message);
        }

        return pagedResponse;
    }

    public async Task<ServiceResponseModel<string>> UpdateMemberAsync(string username, MemberUpdateModel memberUpdate)
    {
        _logger.LogInformation($"Update member... [{username}]");
        ServiceResponseModel<string> serviceResponse = new();

        try
        {
            AppUser appUser = await _unitOfWork.MemberRepository.GetMemberByUsernameAsync(username);
            memberUpdate.ApplyTo(appUser);
            _unitOfWork.MemberRepository.UpdateMember(appUser);

            if (await _unitOfWork.CompleteAsync())
            {
                serviceResponse.Success = true;
                serviceResponse.Data = $"Successfully updated user [{username}]";
                serviceResponse.Message = $"Successfully updated user [{username}]";
                _logger.LogInformation(serviceResponse.Message);
            }
            else
            {
                throw new Exception($"Failed to update user [{username}]");
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
