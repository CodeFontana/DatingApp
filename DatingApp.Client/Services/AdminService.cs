using DatingApp.Client.Http;

namespace DatingApp.Client.Services;

public class AdminService : IAdminService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options;

    public AdminService(IConfiguration config, HttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<ApiResponse<IEnumerable<string>>> GetRolesAsync()
    {
        string apiEndpoint = _config["apiLocation"] + _config["adminEndpoint"] + "/roles";
        using HttpResponseMessage response = await _httpClient.GetAsync(apiEndpoint);
        return await response.Content.ReadApiResponseAsync<IEnumerable<string>>(_options);
    }

    public async Task<ApiResponse<IEnumerable<UserWithRolesResponse>>> GetUsersWithRolesAsync()
    {
        string apiEndpoint = _config["apiLocation"] + _config["adminEndpoint"] + "/users-with-roles";
        using HttpResponseMessage response = await _httpClient.GetAsync(apiEndpoint);
        return await response.Content.ReadApiResponseAsync<IEnumerable<UserWithRolesResponse>>(_options);
    }

    public async Task<ApiResponse<string>> EditRolesAsync(UserWithRolesResponse userWithRoles)
    {
        string apiEndpoint = _config["apiLocation"] + _config["adminEndpoint"] + "/edit-roles";
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(apiEndpoint, userWithRoles);
        return await response.Content.ReadApiResponseAsync<string>(_options);
    }

    public async Task<ApiResponse<bool>> DeleteAccountAsync(string username)
    {
        string apiEndpoint = _config["apiLocation"] + _config["registerEndpoint"] + $"/{username}";
        using HttpResponseMessage response = await _httpClient.DeleteAsync(apiEndpoint);
        return await response.Content.ReadApiResponseAsync<bool>(_options);
    }
}
