namespace API.Services;

public class LikesService : ILikesService
{
    private readonly IMemberRepository _memberRepository;
    private readonly ILikesRepository _likesRepository;
    private readonly ILogger<LikesService> _logger;

    public LikesService(IMemberRepository memberRepository, ILikesRepository likesRepository, ILogger<LikesService> logger)
    {
        _memberRepository = memberRepository;
        _likesRepository = likesRepository;
        _logger = logger;
    }

    public async Task<ServiceResponseModel<IEnumerable<LikeUserModel>>> GetUserLikesAsync(string requestor, string predicate, int sourceUserId)
    {
        ServiceResponseModel<IEnumerable<LikeUserModel>> serviceResponse = new();

        try
        {
            IEnumerable<LikeUserModel> users = await _likesRepository.GetUserLikesAsync(predicate, sourceUserId);

            serviceResponse.Success = true;
            serviceResponse.Data = users;
            serviceResponse.Message = $"Successfully listed likes for [{requestor}]";
            _logger.LogInformation(serviceResponse.Message);
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = $"Failed to list user likes for [{requestor}]";
            _logger.LogError(serviceResponse.Message);
            _logger.LogError(e.Message);
        }

        return serviceResponse;
    }

    public async Task<ServiceResponseModel<string>> ToggleLikeAsync(string requestor, string username, int sourceUserId)
    {
        ServiceResponseModel<string> serviceResponse = new();

        try
        {
            AppUser likedUser = await _memberRepository.GetMemberByUsernameAsync(username);
            AppUser sourceUser = await _likesRepository.GetUserWithLikesAsync(sourceUserId);

            if (likedUser == null)
            {
                throw new Exception($"Liked user not found {username}");
            }

            if (sourceUser.UserName == username)
            {
                throw new Exception($"You cannot like yourself {username}, but we hope you do anyway");
            }

            UserLike userLike = await _likesRepository.GetUserLikeAsync(sourceUserId, likedUser.Id);

            if (userLike != null)
            {
                sourceUser.LikedUsers.Remove(userLike);
            }
            else
            {
                userLike = new()
                {
                    SourceUserId = sourceUserId,
                    LikedUserId = likedUser.Id
                };

                sourceUser.LikedUsers.Add(userLike);
            }

            if (await _memberRepository.SaveAllAsync())
            {
                serviceResponse.Success = true;
                serviceResponse.Data = $"Successfully liked [{username}] on behalf of [{requestor}]";
                serviceResponse.Message = $"Successfully liked [{username}] on behalf of [{requestor}]";
                _logger.LogInformation(serviceResponse.Message);
            }
            else
            {
                throw new Exception($"Failed to like {username}");
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
