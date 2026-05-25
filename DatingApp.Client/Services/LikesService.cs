using DatingApp.Client.Http;

namespace DatingApp.Client.Services;

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

    public Dictionary<string, MemberCacheResponse> LikeListCache { get; set; } = new();

    public async Task<PaginatedResponse<IEnumerable<MemberResponse>>> GetLikesAsync(LikesListQuery likesParameters)
    {
        MemberCacheResponse? cachedData = LikeListCache.GetValueOrDefault(likesParameters.Values);

        if (cachedData?.CacheTime.AddMinutes(5) > DateTime.Now)
        {
            Console.WriteLine($"Found likes list in cache [{likesParameters.Values}]");
            return cachedData.PaginatedResponse;
        }
        else if (cachedData is not null)
        {
            Console.WriteLine($"Like list cache outdated {likesParameters.Values}]");
            LikeListCache.Remove(likesParameters.Values);
        }
        else
        {
            Console.WriteLine($"Like list not in cache [{likesParameters.Values}]");
        }

        string apiEndpoint = _config["apiLocation"] + _config["likesEndpoint"];

        Dictionary<string, string?> queryStringParam = new()
        {
            [nameof(likesParameters.PageNumber)] = likesParameters.PageNumber.ToString(),
            [nameof(likesParameters.PageSize)] = likesParameters.PageSize.ToString(),
            [nameof(likesParameters.Predicate)] = likesParameters.Predicate,
        };

        using HttpResponseMessage response = await _httpClient.GetAsync(QueryHelpers.AddQueryString(apiEndpoint, queryStringParam));
        PaginatedResponse<IEnumerable<MemberResponse>> result =
            await response.Content.ReadPaginatedResponseAsync<IEnumerable<MemberResponse>>(_options);

        result.MetaData = response.Headers.ReadPaginationMetadata(_options);

        if (result.Success)
        {
            MemberCacheResponse cacheResponse = new()
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

    public async Task<ApiResponse<string>> ToggleLikeAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentNullException(nameof(username), "Invalid username");
        }

        string apiEndpoint = _config["apiLocation"] + _config["likesEndpoint"];
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(apiEndpoint, username);
        ApiResponse<string> result = await response.Content.ReadApiResponseAsync<string>(_options);

        LikeListCache = new();

        return result;
    }
}
