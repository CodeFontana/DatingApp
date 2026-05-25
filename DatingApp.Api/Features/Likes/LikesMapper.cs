using DatingApp.Contracts.Likes.Requests;
using DatingApp.Contracts.Members.Responses;
using DatingApp.DataAccess.Internal;
using DatingApp.DataAccess.Pagination;

namespace DatingApp.Api.Features.Likes;

internal static class LikesMapper
{
    public static LikesListCriteria ToCriteria(LikesListQuery query, int userId) => new()
    {
        PageNumber = query.PageNumber,
        PageSize = query.PageSize,
        UserId = userId,
        Predicate = query.Predicate
    };

    public static MemberResponse ToMemberResponse(MemberReadModel model) =>
        Members.MemberMapper.ToResponse(model);
}
