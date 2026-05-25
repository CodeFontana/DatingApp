using DatingApp.Contracts.Members.Requests;
using DatingApp.Contracts.Members.Responses;
using DatingApp.DataAccess.Entities;
using DatingApp.DataAccess.Internal;
using DatingApp.DataAccess.Pagination;

namespace DatingApp.Api.Features.Members;

internal static class MemberMapper
{
    public static MemberListCriteria ToCriteria(MemberListQuery query, string currentUsername) => new()
    {
        PageNumber = query.PageNumber,
        PageSize = query.PageSize,
        CurrentUsername = currentUsername,
        Gender = query.Gender,
        MinAge = query.MinAge,
        MaxAge = query.MaxAge,
        OrderBy = query.OrderBy
    };

    public static MemberResponse ToResponse(MemberReadModel model) => new()
    {
        Id = model.Id,
        Username = model.Username,
        MainPhotoFilename = model.MainPhotoFilename,
        Age = model.Age,
        KnownAs = model.KnownAs,
        Created = model.Created,
        LastActive = model.LastActive,
        Gender = model.Gender,
        Introduction = model.Introduction,
        LookingFor = model.LookingFor,
        Interests = model.Interests,
        City = model.City,
        State = model.State,
        Photos = model.Photos.Select(ToPhotoResponse).ToList(),
        CacheTime = DateTime.UtcNow
    };

    public static PhotoResponse ToPhotoResponse(PhotoReadModel photo) => new()
    {
        Id = photo.Id,
        Filename = photo.Filename,
        IsMain = photo.IsMain
    };

    public static PhotoResponse ToPhotoResponse(Photo photo) => new()
    {
        Id = photo.Id,
        Filename = photo.Filename,
        IsMain = photo.IsMain
    };

    public static void ApplyUpdate(MemberUpdateRequest request, AppUser user)
    {
        user.Introduction = request.Introduction;
        user.LookingFor = request.LookingFor;
        user.Interests = request.Interests;
        user.City = request.City;
        user.State = request.State;
    }

    public static MemberUpdateRequest FromResponse(MemberResponse member) => new()
    {
        Username = member.Username,
        Introduction = member.Introduction,
        LookingFor = member.LookingFor,
        Interests = member.Interests,
        City = member.City,
        State = member.State
    };
}
