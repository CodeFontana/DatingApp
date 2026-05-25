using System.Linq.Expressions;
using DatingApp.DataAccess.Entities;

namespace DatingApp.DataAccess.Internal;

public sealed class PhotoReadModel
{
    public int Id { get; set; }
    public string Filename { get; set; } = string.Empty;
    public bool IsMain { get; set; }

    public static readonly Expression<Func<Photo, PhotoReadModel>> Projection = p => new PhotoReadModel
    {
        Id = p.Id,
        Filename = p.Filename,
        IsMain = p.IsMain
    };

    public static PhotoReadModel FromEntity(Photo photo)
    {
        ArgumentNullException.ThrowIfNull(photo);
        return new PhotoReadModel
        {
            Id = photo.Id,
            Filename = photo.Filename,
            IsMain = photo.IsMain
        };
    }
}
