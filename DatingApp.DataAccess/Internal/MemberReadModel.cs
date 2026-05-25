using System.Linq.Expressions;
using DatingApp.DataAccess.Entities;
using DatingApp.DataAccess.Extensions;

namespace DatingApp.DataAccess.Internal;

public sealed class MemberReadModel
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string MainPhotoFilename { get; set; } = string.Empty;
    public int Age { get; set; }
    public string KnownAs { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public DateTime LastActive { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Introduction { get; set; } = string.Empty;
    public string LookingFor { get; set; } = string.Empty;
    public string Interests { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public IList<PhotoReadModel> Photos { get; set; } = [];

    public static readonly Expression<Func<AppUser, MemberReadModel>> Projection = u => new MemberReadModel
    {
        Id = u.Id,
        Username = u.UserName ?? string.Empty,
        MainPhotoFilename = u.Photos.Where(p => p.IsMain).Select(p => p.Filename).FirstOrDefault() ?? string.Empty,
        Age = u.DateOfBirth.CalculateAge(),
        KnownAs = u.KnownAs,
        Created = u.Created,
        LastActive = u.LastActive,
        Gender = u.Gender,
        Introduction = u.Introduction,
        LookingFor = u.LookingFor,
        Interests = u.Interests,
        City = u.City,
        State = u.State,
        Photos = u.Photos.Select(p => new PhotoReadModel
        {
            Id = p.Id,
            Filename = p.Filename,
            IsMain = p.IsMain
        }).ToList()
    };

    public static MemberReadModel FromEntity(AppUser user)
    {
        ArgumentNullException.ThrowIfNull(user);
        return new MemberReadModel
        {
            Id = user.Id,
            Username = user.UserName ?? string.Empty,
            MainPhotoFilename = user.Photos?.FirstOrDefault(x => x.IsMain)?.Filename ?? string.Empty,
            Age = user.DateOfBirth.CalculateAge(),
            KnownAs = user.KnownAs,
            Created = user.Created,
            LastActive = user.LastActive,
            Gender = user.Gender,
            Introduction = user.Introduction,
            LookingFor = user.LookingFor,
            Interests = user.Interests,
            City = user.City,
            State = user.State,
            Photos = user.Photos?.Select(PhotoReadModel.FromEntity).ToList() ?? []
        };
    }
}
