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
        string apiEndpoint = _config["apiLocation"] + _config["loginEndpoint"];
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(apiEndpoint, loginUser);
        ServiceResponseModel<AuthUserModel> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<AuthUserModel>>(_options);

        if (result.Success)
        {
            await ((AuthStateProvider)_authStateProvider).NotifyUserAuthenticationAsync(result.Data.Token);
        }

        return result;
    }

    public async Task<ServiceResponseModel<AuthUserModel>> RegisterAsync(RegisterUserModel registerUser)
    {
        string apiEndpoint = _config["apiLocation"] + _config["registerEndpoint"];
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(apiEndpoint, registerUser);
        return await response.Content.ReadFromJsonAsync<ServiceResponseModel<AuthUserModel>>(_options);
    }

    public async Task LogoutAsync()
    {
        await ((AuthStateProvider)_authStateProvider).NotifyUserLogoutAsync();
    }
}
