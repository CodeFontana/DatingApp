namespace DataAccessLibrary.Entities;

public class AppRole : IdentityRole<int>
{
    [InverseProperty(nameof(AppUserRole.Role))]
    public ICollection<AppUserRole> UserRoles { get; set; } = [];
}
