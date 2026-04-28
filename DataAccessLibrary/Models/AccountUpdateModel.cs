namespace DataAccessLibrary.Models;

public class AccountUpdateModel
{
    public int Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}
