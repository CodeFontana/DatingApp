using System.Net.Http.Json;
using System.Security.Cryptography;

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

    public async Task<PaginationResponseModel<IEnumerable<UserWithRolesModel>>> GetUsersWithRolesAsync(PaginationParameters pageParameters)
    {
        string apiEndpoint = _config["apiLocation"] + _config["adminEndpoint"] + "/users-with-roles";

        Dictionary<string, string> queryStringParam = new()
        {
            [nameof(pageParameters.PageNumber)] = pageParameters.PageNumber.ToString(),
            [nameof(pageParameters.PageSize)] = pageParameters.PageSize.ToString()
        };

        using HttpResponseMessage response = await _httpClient.GetAsync(QueryHelpers.AddQueryString(apiEndpoint, queryStringParam));
        PaginationResponseModel<IEnumerable<UserWithRolesModel>> result = await response.Content.ReadFromJsonAsync<PaginationResponseModel<IEnumerable<UserWithRolesModel>>>(_options);

        if (response.Headers != null && response.Headers.Contains("Pagination"))
        {
            result.MetaData = JsonSerializer.Deserialize<PaginationModel>(response.Headers.GetValues("Pagination").First(), _options);
        }

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
