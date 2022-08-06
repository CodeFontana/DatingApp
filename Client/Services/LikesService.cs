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

    public Dictionary<string, MemberCacheModel> LikeListCache { get; set; } = new();

    public async Task<PaginationResponseModel<IEnumerable<MemberModel>>> GetLikesAsync(LikesParameters likesParameters)
    {
        MemberCacheModel cachedData = LikeListCache.GetValueOrDefault(likesParameters.Values);

        if (cachedData?.CacheTime.AddMinutes(5) > DateTime.Now)
        {
            Console.WriteLine($"Found likes list in cache [{likesParameters.Values}]");
            return cachedData.PaginatedResponse;
        }
        else if (cachedData != null)
        {
            Console.WriteLine($"Like list cache outdated {likesParameters.Values}]");
            LikeListCache.Remove(likesParameters.Values);
        }
        else
        {
            Console.WriteLine($"Like list not in cache [{likesParameters.Values}]");
        }

        string apiEndpoint = _config["apiLocation"] + _config["likesEndpoint"];

        Dictionary<string, string> queryStringParam = new()
        {
            [nameof(likesParameters.PageNumber)] = likesParameters.PageNumber.ToString(),
            [nameof(likesParameters.PageSize)] = likesParameters.PageSize.ToString(),
            [nameof(likesParameters.UserId)] = likesParameters.UserId.ToString(),
            [nameof(likesParameters.Predicate)] = likesParameters.Predicate.ToString(),
        };

        using HttpResponseMessage response = await _httpClient.GetAsync(QueryHelpers.AddQueryString(apiEndpoint, queryStringParam));
        PaginationResponseModel<IEnumerable<MemberModel>> result = await response.Content.ReadFromJsonAsync<PaginationResponseModel<IEnumerable<MemberModel>>>(_options);

        if (response.Headers != null && response.Headers.Contains("Pagination"))
        {
            result.MetaData = JsonSerializer.Deserialize<PaginationModel>(response.Headers.GetValues("Pagination").First(), _options);
        }

        if (result.Success)
        {
            MemberCacheModel cacheResponse = new()
            {
                CacheTime = DateTime.Now,
                SearchKey = likesParameters.Values,
                PaginatedResponse = result
            };

            LikeListCache.Remove(likesParameters.Values);
            LikeListCache.TryAdd(likesParameters.Values, cacheResponse);
        }

        return result;
    }

    public async Task<ServiceResponseModel<string>> ToggleLikeAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentNullException(nameof(username), "Invalid username");
        }

        string apiEndpoint = _config["apiLocation"] + _config["likesEndpoint"];
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(apiEndpoint, username);
        ServiceResponseModel<string> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<string>>(_options);

        LikeListCache = new();

        return result;
    }
}
