namespace Client.Services;

public class AdminService : IAdminService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options;

    public AdminService(IConfiguration config,
                        HttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<ServiceResponseModel<IEnumerable<UserWithRolesModel>>> GetUsersWithRolesAsync()
    {
        string apiEndpoint = _config["apiLocation"] + _config["adminEndpoint"] + "/users-with-roles";
        using HttpResponseMessage response = await _httpClient.GetAsync(apiEndpoint);
        ServiceResponseModel<IEnumerable<UserWithRolesModel>> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<IEnumerable<UserWithRolesModel>>>(_options);
        return result;
    }

    public async Task<ServiceResponseModel<string>> EditRolesAsync(UserWithRolesModel userWithRoles)
    {
        string apiEndpoint = _config["apiLocation"] + _config["adminEndpoint"] + "/edit-roles";
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(apiEndpoint, userWithRoles);
        ServiceResponseModel<string> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<string>> (_options);
        return result;
    }
}
