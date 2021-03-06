namespace API.Services;

public class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<MemberService> _logger;

    public MemberService(IMemberRepository memberRepository, IMapper mapper, ILogger<MemberService> logger)
    {
        _memberRepository = memberRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ServiceResponseModel<MemberModel>> GetMemberAsync(string username, string requestor)
    {
        _logger.LogInformation($"Get member {username}... [{requestor}]");
        ServiceResponseModel<MemberModel> serviceResponse = new();

        try
        {
            serviceResponse.Success = true;
            serviceResponse.Data = await _memberRepository.GetMemberAsync(username);
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
            PaginationList<MemberModel> data = await _memberRepository.GetMembersAsync(userParameters);

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
            AppUser appUser = await _memberRepository.GetMemberByUsernameAsync(username);
            _mapper.Map(memberUpdate, appUser);
            _memberRepository.UpdateMember(appUser);

            if (await _memberRepository.SaveAllAsync())
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
