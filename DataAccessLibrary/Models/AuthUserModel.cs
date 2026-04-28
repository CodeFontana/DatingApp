namespace DataAccessLibrary.Models;

public record AuthUserModel
{
    public string Username { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
}