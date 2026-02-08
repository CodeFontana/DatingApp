using System.Linq.Expressions;

namespace DataAccessLibrary.Models;

public sealed class MemberModel
{
    public int Id { get; set; }

    public string Username { get; set; }

    public string MainPhotoFilename { get; set; }

    public int Age { get; set; }

    [MaxLength(50)]
    public string KnownAs { get; set; }

    public DateTime Created { get; set; }

    public DateTime LastActive { get; set; }

    [MaxLength(25)]
    public string Gender { get; set; }

    [MaxLength(1000)]
    public string Introduction { get; set; }

    [MaxLength(1000)]
    public string LookingFor { get; set; }

    [MaxLength(1000)]
    public string Interests { get; set; }

    [MaxLength(100)]
    public string City { get; set; }

    [MaxLength(100)]
    public string State { get; set; }

    public IList<PhotoModel> Photos { get; set; }

    public DateTime CacheTime { get; set; }

    public static readonly Expression<Func<AppUser, MemberModel>> Projection = u => new MemberModel
    {
        Id = u.Id,
        Username = u.UserName,
        MainPhotoFilename = u.Photos.Where(p => p.IsMain).Select(p => p.Filename).FirstOrDefault(),
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
        Photos = u.Photos.Select(p => new PhotoModel
        {
            Id = p.Id,
            Filename = p.Filename,
            IsMain = p.IsMain
        }).ToList()
    };

    public static MemberModel FromEntity(AppUser user)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return new MemberModel
        {
            Id = user.Id,
            Username = user.UserName,
            MainPhotoFilename = user.Photos?.FirstOrDefault(x => x.IsMain)?.Filename,
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
            Photos = user.Photos?.Select(PhotoModel.FromEntity).ToList(),
            CacheTime = DateTime.UtcNow
        };
    }
}
