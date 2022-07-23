namespace DataAccessLibrary.Models;

public class UserWithRolesModel
{
    public int Id { get; set; }
    public string Username { get; set; }
    public List<string> Roles { get; set; }
}
