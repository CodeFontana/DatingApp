namespace Client.Authentication;

public interface IAuthenticationService
{
    Task<ServiceResponseModel<AuthUserModel>> LoginAsync(LoginUserModel loginUser);
    Task LogoutAsync();
    Task<ServiceResponseModel<AuthUserModel>> RegisterAsync(RegisterUserModel registerUser);
}