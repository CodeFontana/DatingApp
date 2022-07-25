namespace Client.Services;

public class LikesService : ILikesService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options;

    public LikesService(IConfiguration config,
                        HttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<ServiceResponseModel<string>> ToggleLikeAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentNullException("Invalid username");
        }

        string apiEndpoint = _config["apiLocation"] + _config["likesEndpoint"];
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(apiEndpoint, username);
        ServiceResponseModel<string> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<string>>(_options);

        return result;
    }

    public async Task<ServiceResponseModel<IEnumerable<LikeUserModel>>> GetLikesAsync(string predicate)
    {
        if (string.IsNullOrWhiteSpace(predicate))
        {
            throw new ArgumentNullException("Invalid predicate");
        }

        Dictionary<string, string> queryStringParam = new()
        {
            ["likes"] = predicate
        };

        string apiEndpoint = _config["apiLocation"] + _config["likesEndpoint"];
        using HttpResponseMessage response = await _httpClient.GetAsync(apiEndpoint);
        ServiceResponseModel<IEnumerable<LikeUserModel>> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<IEnumerable<LikeUserModel>>>(_options);

        return result;
    }
}
