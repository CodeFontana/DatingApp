namespace Client.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options;
    private readonly AuthenticationStateProvider _authStateProvider;

    public AuthenticationService(IConfiguration config,
                                 HttpClient httpClient,
                                 AuthenticationStateProvider authStateProvider)
    {
        _config = config;
        _httpClient = httpClient;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _authStateProvider = authStateProvider;
    }

    public async Task<ServiceResponseModel<AuthUserModel>> LoginAsync(LoginUserModel loginUser)
    {
        ServiceResponseModel<AuthUserModel> result = new();

        try
        {
            string apiEndpoint = _config["apiLocation"] + _config["loginEndpoint"];
            using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(apiEndpoint, loginUser);
            result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<AuthUserModel>>(_options);

            if (result.Success)
            {
                await ((JwtAuthenticationStateProvider)_authStateProvider).NotifyUserAuthenticationAsync(result.Data.Token);
            }
        }
        catch (Exception e)
        {
            result.Success = false;
            result.Message = e.Message;
        }

        return result;
    }

    public async Task<ServiceResponseModel<AuthUserModel>> RegisterAsync(RegisterUserModel registerUser)
    {
        ServiceResponseModel<AuthUserModel> result = new();

        try
        {
            string apiEndpoint = _config["apiLocation"] + _config["registerEndpoint"];
            using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(apiEndpoint, registerUser);
            result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<AuthUserModel>>(_options);

            if (result.Success)
            {
                await ((JwtAuthenticationStateProvider)_authStateProvider).NotifyUserAuthenticationAsync(result.Data.Token);
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
