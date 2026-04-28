namespace DataAccessLibrary.Entities;

[Table("UserLike")]
[PrimaryKey(nameof(SourceUserId), nameof(LikedUserId))]
public class UserLike
{
    [ForeignKey(nameof(SourceUserId))]
    [InverseProperty(nameof(AppUser.LikedUsers))]
    public AppUser SourceUser { get; set; } = null!;

    public int SourceUserId { get; set; }

    [ForeignKey(nameof(LikedUserId))]
    [InverseProperty(nameof(AppUser.LikedByUsers))]
    public AppUser LikedUser { get; set; } = null!;

    public int LikedUserId { get; set; }
}
