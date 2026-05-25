namespace DatingApp.Contracts.Authentication.Responses;

public class AccountResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public DateTime LastActive { get; set; }
}
