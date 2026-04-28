namespace DataAccessLibrary.Entities;

public class AppUser : IdentityUser<int>
{
    [InverseProperty(nameof(AppUserRole.User))]
    public ICollection<AppUserRole> UserRoles { get; set; } = [];

    public DateTime DateOfBirth { get; set; }

    [MaxLength(50)]
    public string KnownAs { get; set; } = string.Empty;

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public DateTime LastActive { get; set; } = DateTime.UtcNow;

    [MaxLength(25)]
    public string Gender { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Introduction { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string LookingFor { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Interests { get; set; } = string.Empty;

    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [MaxLength(100)]
    public string State { get; set; } = string.Empty;

    public ICollection<Photo> Photos { get; set; } = [];

    [InverseProperty(nameof(UserLike.SourceUser))]
    public ICollection<UserLike> LikedUsers { get; set; } = [];

    [InverseProperty(nameof(UserLike.LikedUser))]
    public ICollection<UserLike> LikedByUsers { get; set; } = [];

    [InverseProperty(nameof(Message.Sender))]
    public ICollection<Message> MessagesSent { get; set; } = [];

    [InverseProperty(nameof(Message.Recipient))]
    public ICollection<Message> MessagesReceived { get; set; } = [];
}
