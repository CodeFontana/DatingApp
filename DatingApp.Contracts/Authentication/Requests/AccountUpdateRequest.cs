namespace DatingApp.Contracts.Authentication.Requests;

public class AccountUpdateRequest
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
