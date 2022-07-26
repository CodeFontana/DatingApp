namespace DataAccessLibrary.Entities;

public class AppUser : IdentityUser<int>
{
    public ICollection<AppUserRole> UserRoles { get; set; }
    
    public DateTime DateOfBirth { get; set; }

    [MaxLength(50)]
    public string KnownAs { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public DateTime LastActive { get; set; } = DateTime.UtcNow;

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

    public ICollection<Photo> Photos { get; set; }

    public ICollection<UserLike> LikedUsers { get; set; }

    public ICollection<UserLike> LikedByUsers { get; set; }
    public ICollection<Message> MessagesSent { get; set; }
    public ICollection<Message> MessagesReceived { get; set; }
}
