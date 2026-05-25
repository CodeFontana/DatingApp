using DatingApp.Client.Http;

namespace DatingApp.Client.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options;
    private readonly AuthenticationStateProvider _authStateProvider;

    public AuthenticationService(
        IConfiguration config,
        HttpClient httpClient,
        AuthenticationStateProvider authStateProvider)
    {
        _config = config;
        _httpClient = httpClient;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _authStateProvider = authStateProvider;
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest loginUser)
    {
        ApiResponse<AuthResponse> result = new();

        try
        {
            string apiEndpoint = _config["apiLocation"] + _config["loginEndpoint"];
            using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(apiEndpoint, loginUser);
            result = await response.Content.ReadApiResponseAsync<AuthResponse>(_options);

            if (result.Success && result.Data is not null)
            {
                await ((JwtAuthenticationStateProvider)_authStateProvider)
                    .NotifyUserAuthenticationAsync(result.Data.Token);
            }
        }
        catch (Exception e)
        {
            result.Success = false;
            result.Message = e.Message;
        }

        return result;
    }

    public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest registerUser)
    {
        ApiResponse<AuthResponse> result = new();

        try
        {
            string apiEndpoint = _config["apiLocation"] + _config["registerEndpoint"];
            using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(apiEndpoint, registerUser);
            result = await response.Content.ReadApiResponseAsync<AuthResponse>(_options);

            if (result.Success && result.Data is not null)
            {
                await ((JwtAuthenticationStateProvider)_authStateProvider)
                    .NotifyUserAuthenticationAsync(result.Data.Token);
            }
        }
        catch (Exception e)
        {
            result.Success = false;
            result.Message = e.Message;
        }

        return result;
    }

    public async Task LogoutAsync()
    {
        await ((JwtAuthenticationStateProvider)_authStateProvider).NotifyUserLogoutAsync();
    }
}
