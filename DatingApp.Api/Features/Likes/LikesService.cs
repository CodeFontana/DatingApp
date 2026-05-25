using DatingApp.Api.Features.Common;
using DatingApp.Api.Features.Members;
using DatingApp.Contracts.Common;
using DatingApp.Contracts.Likes.Requests;
using DatingApp.Contracts.Members.Responses;
using DatingApp.DataAccess.Entities;
using DatingApp.DataAccess.Interfaces;
using DatingApp.DataAccess.Internal;
using DatingApp.DataAccess.Pagination;

namespace DatingApp.Api.Features.Likes;

public class LikesService : ILikesService
{
    private readonly ILogger<LikesService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public LikesService(ILogger<LikesService> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResponse<IEnumerable<MemberResponse>>> GetUserLikesAsync(string requestor, int userId, LikesListQuery query)
    {
        _logger.LogInformation("Get likes... [{Requestor}]", requestor);
        PaginatedResponse<IEnumerable<MemberResponse>> pagedResponse = new();

        try
        {
            PaginationList<MemberReadModel> data = await _unitOfWork.LikesRepository.GetUserLikesAsync(
                LikesMapper.ToCriteria(query, userId));

            pagedResponse.Success = true;
            pagedResponse.Data = data.Select(MemberMapper.ToResponse);
            pagedResponse.MetaData = PaginationMapper.ToContract(data.MetaData);
            pagedResponse.Message = $"Successfully listed likes for [{requestor}]";
            _logger.LogInformation(pagedResponse.Message);
        }
        catch (Exception e)
        {
            pagedResponse.Success = false;
            pagedResponse.Message = $"Failed to list user likes for [{requestor}]";
            _logger.LogError(e, pagedResponse.Message);
        }

        return pagedResponse;
    }

    public async Task<ApiResponse<string>> ToggleLikeAsync(string requestor, string username, int sourceUserId)
    {
        _logger.LogInformation("Toggle like for {Username}... [{Requestor}]", username, requestor);
        ApiResponse<string> response = new();

        try
        {
            AppUser likedUser = await _unitOfWork.MemberRepository.GetMemberByUsernameAsync(username)
                ?? throw new Exception($"Liked user not found {username}");
            AppUser sourceUser = await _unitOfWork.LikesRepository.GetUserWithLikesAsync(sourceUserId)
                ?? throw new Exception($"Source user not found {sourceUserId}");

            if (sourceUser.UserName == username)
            {
                throw new Exception($"You cannot like yourself {username}, but we hope you do anyway");
            }

            UserLike? userLike = await _unitOfWork.LikesRepository.GetUserLikeAsync(sourceUserId, likedUser.Id);

            if (userLike != null)
            {
                sourceUser.LikedUsers.Remove(userLike);
                response.Data = $"Unliked {username}";
                response.Message = $"Successfully unliked [{username}] on behalf of [{requestor}]";
            }
            else
            {
                sourceUser.LikedUsers.Add(new UserLike
                {
                    SourceUserId = sourceUserId,
                    LikedUserId = likedUser.Id
                });
                response.Data = $"Liked {username}";
                response.Message = $"Successfully liked [{username}] on behalf of [{requestor}]";
            }

            if (await _unitOfWork.CompleteAsync())
            {
                response.Success = true;
                _logger.LogInformation(response.Message);
            }
            else
            {
                throw new Exception($"Failed to toggle like status for {username}");
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
