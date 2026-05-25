using DatingApp.Client.Http;

namespace DatingApp.Client.Services;

public class MemberService : IMemberService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options;

    public MemberService(IConfiguration config,
                         HttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public List<MemberResponse> MemberCache { get; set; } = new();
    public Dictionary<string, MemberCacheResponse> MemberListCache { get; set; } = new();

    public async Task<ApiResponse<MemberResponse>> GetMemberAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentNullException(nameof(username), "Invalid username");
        }

        MemberResponse? member = MemberCache.FirstOrDefault(m => m.Username.Equals(username));

        if (member is not null && member.CacheTime.AddMinutes(5) > DateTime.Now)
        {
            Console.WriteLine($"Member found in cache [{username}]");

            return new ApiResponse<MemberResponse>()
            {
                Success = true,
                Data = member,
                Message = "Member cache"
            };
        }
        else if (member is not null)
        {
            Console.WriteLine($"Remove outdated member from cache [{username}]");
            MemberCache.Remove(member);
        }

        Console.WriteLine($"Member not found in cache [{username}]");

        string apiEndpoint = _config["apiLocation"] + _config["membersEndpoint"] + $"/{username}";
        using HttpResponseMessage response = await _httpClient.GetAsync(apiEndpoint);
        ApiResponse<MemberResponse> result = await response.Content.ReadApiResponseAsync<MemberResponse>(_options);

        if (result.Success && result.Data is not null)
        {
            member = result.Data;
            member.CacheTime = DateTime.Now;
            MemberCache.Add(member);
        }

        return result;
    }

    public async Task<PaginatedResponse<IEnumerable<MemberResponse>>> GetMembersAsync(MemberListQuery memberParameters)
    {
        MemberCacheResponse? cachedData = MemberListCache.GetValueOrDefault(memberParameters.Values);

        if (cachedData?.CacheTime.AddMinutes(5) > DateTime.Now)
        {
            Console.WriteLine($"Found member list in cache [{memberParameters.Values}]");
            return cachedData.PaginatedResponse;
        }
        else if (cachedData is not null)
        {
            Console.WriteLine($"Member list cache outdated {memberParameters.Values}]");
            MemberListCache.Remove(memberParameters.Values);
        }
        else
        {
            Console.WriteLine($"Member list not in cache [{memberParameters.Values}]");
        }

        string apiEndpoint = _config["apiLocation"] + _config["membersEndpoint"];

        Dictionary<string, string?> queryStringParam = new()
        {
            [nameof(memberParameters.PageNumber)] = memberParameters.PageNumber.ToString(),
            [nameof(memberParameters.PageSize)] = memberParameters.PageSize.ToString(),
            [nameof(memberParameters.MinAge)] = memberParameters.MinAge.ToString(),
            [nameof(memberParameters.MaxAge)] = memberParameters.MaxAge.ToString(),
            [nameof(memberParameters.Gender)] = memberParameters.Gender,
            [nameof(memberParameters.OrderBy)] = memberParameters.OrderBy
        };

        using HttpResponseMessage response = await _httpClient.GetAsync(QueryHelpers.AddQueryString(apiEndpoint, queryStringParam));
        PaginatedResponse<IEnumerable<MemberResponse>> result =
            await response.Content.ReadPaginatedResponseAsync<IEnumerable<MemberResponse>>(_options);

        result.MetaData = response.Headers.ReadPaginationMetadata(_options);

        if (result.Success)
        {
            MemberCacheResponse cacheResponse = new MemberCacheResponse
            {
                CacheTime = DateTime.Now,
                SearchKey = memberParameters.Values,
                PaginatedResponse = result
            };

            MemberListCache.Remove(memberParameters.Values);
            MemberListCache.TryAdd(memberParameters.Values, cacheResponse);
        }

        return result;
    }

    public async Task<ApiResponse<string>> UpdateMemberAsync(MemberUpdateRequest memberUpdate)
    {
        string apiEndpoint = _config["apiLocation"] + _config["membersEndpoint"];
        using HttpResponseMessage response = await _httpClient.PutAsJsonAsync(apiEndpoint, memberUpdate);
        return await response.Content.ReadApiResponseAsync<string>(_options);
    }
}
