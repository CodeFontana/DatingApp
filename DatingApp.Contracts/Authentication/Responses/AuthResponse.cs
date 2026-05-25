namespace DatingApp.Contracts.Authentication.Responses;

public record AuthResponse
{
    public string Username { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
}
