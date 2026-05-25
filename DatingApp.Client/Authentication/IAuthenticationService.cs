namespace DatingApp.Client.Authentication;

public interface IAuthenticationService
{
    Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest loginUser);
    Task LogoutAsync();
    Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest registerUser);
}