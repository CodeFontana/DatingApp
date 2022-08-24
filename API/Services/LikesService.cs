namespace API.Services;

public class LikesService : ILikesService
{
    private readonly ILogger<LikesService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public LikesService(ILogger<LikesService> logger,
                        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginationResponseModel<PaginationList<MemberModel>>> GetUserLikesAsync(string requestor, LikesParameters likesParameters)
    {
        _logger.LogInformation($"Get likes... [{requestor}]");
        PaginationResponseModel<PaginationList<MemberModel>> pagedResponse = new();

        try
        {
            PaginationList<MemberModel> data = await _unitOfWork.LikesRepository.GetUserLikesAsync(likesParameters);

            pagedResponse.Success = true;
            pagedResponse.Data = data;
            pagedResponse.MetaData = data.MetaData;
            pagedResponse.Message = $"Successfully listed likes for [{requestor}]";
            _logger.LogInformation(pagedResponse.Message);
        }
        catch (Exception e)
        {
            pagedResponse.Success = false;
            pagedResponse.Message = $"Failed to list user likes for [{requestor}]";
            _logger.LogError(pagedResponse.Message);
            _logger.LogError(e.Message);
        }

        return pagedResponse;
    }

    public async Task<ServiceResponseModel<string>> ToggleLikeAsync(string requestor, string username, int sourceUserId)
    {
        _logger.LogInformation($"Toggle like for {username}... [{requestor}]");
        ServiceResponseModel<string> serviceResponse = new();

        try
        {
            AppUser likedUser = await _unitOfWork.MemberRepository.GetMemberByUsernameAsync(username);
            AppUser sourceUser = await _unitOfWork.LikesRepository.GetUserWithLikesAsync(sourceUserId);

            if (likedUser == null)
            {
                throw new Exception($"Liked user not found {username}");
            }

            if (sourceUser.UserName == username)
            {
                throw new Exception($"You cannot like yourself {username}, but we hope you do anyway");
            }

            UserLike userLike = await _unitOfWork.LikesRepository.GetUserLikeAsync(sourceUserId, likedUser.Id);

            if (userLike != null)
            {
                sourceUser.LikedUsers.Remove(userLike);
                serviceResponse.Data = $"Unliked {username}";
                serviceResponse.Message = $"Successfully unliked [{username}] on behalf of [{requestor}]";
            }
            else
            {
                userLike = new()
                {
                    SourceUserId = sourceUserId,
                    LikedUserId = likedUser.Id
                };

                sourceUser.LikedUsers.Add(userLike);
                serviceResponse.Data = $"Liked {username}";
                serviceResponse.Message = $"Successfully liked [{username}] on behalf of [{requestor}]";
            }

            if (await _unitOfWork.Complete())
            {
                serviceResponse.Success = true;
                _logger.LogInformation(serviceResponse.Message);
            }
            else
            {
                throw new Exception($"Failed to toggle like status for {username}");
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
