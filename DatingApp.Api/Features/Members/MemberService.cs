using DatingApp.Api.Features.Common;
using DatingApp.Contracts.Common;
using DatingApp.Contracts.Members.Requests;
using DatingApp.Contracts.Members.Responses;
using DatingApp.DataAccess.Entities;
using DatingApp.DataAccess.Interfaces;
using DatingApp.DataAccess.Internal;
using DatingApp.DataAccess.Pagination;

namespace DatingApp.Api.Features.Members;

public sealed class MemberService : IMemberService
{
    private readonly ILogger<MemberService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public MemberService(ILogger<MemberService> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<MemberResponse>> GetMemberAsync(string username, string requestor)
    {
        _logger.LogInformation("Get member {Username}... [{Requestor}]", username, requestor);
        ApiResponse<MemberResponse> response = new();

        try
        {
            MemberReadModel? member = await _unitOfWork.MemberRepository.GetMemberAsync(username);
            response.Success = member is not null;
            response.Data = member is null ? null : MemberMapper.ToResponse(member);
            response.Message = response.Success
                ? $"Successfully retrieved [{username}] for [{requestor}]"
                : $"Failed to retrieve [{username}] for [{requestor}]";
            _logger.LogInformation(response.Message);
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = $"Failed to retrieve [{username}] for [{requestor}]";
            _logger.LogError(e, response.Message);
        }

        return response;
    }

    public async Task<PaginatedResponse<IEnumerable<MemberResponse>>> GetMembersAsync(string requestor, MemberListQuery query)
    {
        _logger.LogInformation("Get members... [{Requestor}]", requestor);
        PaginatedResponse<IEnumerable<MemberResponse>> pagedResponse = new();

        try
        {
            PaginationList<MemberReadModel> data = await _unitOfWork.MemberRepository.GetMembersAsync(
                MemberMapper.ToCriteria(query, requestor));

            pagedResponse.Success = true;
            pagedResponse.Data = data.Select(MemberMapper.ToResponse);
            pagedResponse.MetaData = PaginationMapper.ToContract(data.MetaData);
            pagedResponse.Message = $"Successfully listed users for [{requestor}]";
            _logger.LogInformation(pagedResponse.Message);
        }
        catch (Exception e)
        {
            pagedResponse.Success = false;
            pagedResponse.Message = $"Failed to list users for [{requestor}]";
            _logger.LogError(e, pagedResponse.Message);
        }

        return pagedResponse;
    }

    public async Task<ApiResponse<string>> UpdateMemberAsync(string username, MemberUpdateRequest memberUpdate)
    {
        _logger.LogInformation("Update member... [{Username}]", username);
        ApiResponse<string> response = new();

        try
        {
            AppUser appUser = await _unitOfWork.MemberRepository.GetMemberByUsernameAsync(username)
                ?? throw new Exception($"User not found [{username}]");

            MemberMapper.ApplyUpdate(memberUpdate, appUser);
            _unitOfWork.MemberRepository.UpdateMember(appUser);

            if (await _unitOfWork.CompleteAsync())
            {
                response.Success = true;
                response.Data = $"Successfully updated user [{username}]";
                response.Message = response.Data;
                _logger.LogInformation(response.Message);
            }
            else
            {
                throw new Exception($"Failed to update user [{username}]");
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
