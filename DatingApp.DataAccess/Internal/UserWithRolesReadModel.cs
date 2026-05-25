namespace DatingApp.DataAccess.Internal;

public sealed class UserWithRolesReadModel
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
}
