namespace DataAccessLibrary.Models;

public class UserWithRolesModel
{
    public int Id { get; set; }

    [Required]
    public string Username { get; set; }

    public List<string> Roles { get; set; }
}
