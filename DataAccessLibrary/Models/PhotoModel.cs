namespace DataAccessLibrary.Models;

using System;
using System.Linq.Expressions;
using DataAccessLibrary.Entities;

public class PhotoModel
{
    public int Id { get; set; }
    public string Filename { get; set; }
    public bool IsMain { get; set; }

    public static readonly Expression<Func<Photo, PhotoModel>> Projection = p => new PhotoModel
    {
        Id = p.Id,
        Filename = p.Filename,
        IsMain = p.IsMain
    };

    public static PhotoModel FromEntity(Photo photo)
    {
        ArgumentNullException.ThrowIfNull(photo);

        return new PhotoModel
        {
            Id = photo.Id,
            Filename = photo.Filename,
            IsMain = photo.IsMain
        };
    }
}
