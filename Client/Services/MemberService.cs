namespace Client.Services;

public class MemberService : IMemberService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options;
    private readonly IMapper _mapper;

    public MemberService(IConfiguration config,
                         HttpClient httpClient,
                         IMapper mapper)
    {
        _config = config;
        _httpClient = httpClient;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _mapper = mapper;
    }

    public List<MemberModel> MemberCache { get; set; } = new();
    public Dictionary<string, MemberCacheModel> MemberListCache { get; set; } = new();

    public async Task<ServiceResponseModel<MemberModel>> GetMemberAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentNullException("Invalid username");
        }

        MemberModel member = MemberCache.FirstOrDefault(m => m.Username.Equals(username));

        if (member != null && member.CacheTime.AddMinutes(5) > DateTime.Now)
        {
            Console.WriteLine($"Member found in cache [{username}]");

            return new ServiceResponseModel<MemberModel>()
            {
                Success = true,
                Data = member,
                Message = "Member cache"
            };
        }
        else if (member != null)
        {
            Console.WriteLine($"Remove outdated member from cache [{username}]");
            MemberCache.Remove(member);
        }

        Console.WriteLine($"Member not found in cache [{username}]");

        string apiEndpoint = _config["apiLocation"] + _config["membersEndpoint"] + $"/{username}";
        using HttpResponseMessage response = await _httpClient.GetAsync(apiEndpoint);
        ServiceResponseModel<MemberModel> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<MemberModel>>(_options);

        if (result.Success)
        {
            member = result.Data;
            member.CacheTime = DateTime.Now;
            MemberCache.Add(member);
        }
        
        return result;
    }

    public async Task<PaginationResponseModel<IEnumerable<MemberModel>>> GetMembersAsync(MemberParameters memberParameters)
    {
        MemberCacheModel cachedData = MemberListCache.GetValueOrDefault(memberParameters.Values);

        if (cachedData?.CacheTime.AddMinutes(5) > DateTime.Now)
        {
            Console.WriteLine($"Found member list in cache [{memberParameters.Values}]");
            return cachedData.PaginatedResponse;
        }
        else if (cachedData != null)
        {
            Console.WriteLine($"Member list cache outdated {memberParameters.Values}]");
            MemberListCache.Remove(memberParameters.Values);
        }
        else
        {
            Console.WriteLine($"Member list not in cache [{memberParameters.Values}]");
        }

        string apiEndpoint = _config["apiLocation"] + _config["membersEndpoint"];

        Dictionary<string, string> queryStringParam = new()
        {
            [nameof(memberParameters.PageNumber)] = memberParameters.PageNumber.ToString(),
            [nameof(memberParameters.PageSize)] = memberParameters.PageSize.ToString(),
            [nameof(memberParameters.MinAge)] = memberParameters.MinAge.ToString(),
            [nameof(memberParameters.MaxAge)] = memberParameters.MaxAge.ToString(),
            [nameof(memberParameters.Gender)] = memberParameters.Gender,
            [nameof(memberParameters.OrderBy)] = memberParameters.OrderBy
        };

        using HttpResponseMessage response = await _httpClient.GetAsync(QueryHelpers.AddQueryString(apiEndpoint, queryStringParam));
        PaginationResponseModel<IEnumerable<MemberModel>> result = await response.Content.ReadFromJsonAsync<PaginationResponseModel<IEnumerable<MemberModel>>>(_options);

        if (response.Headers != null && response.Headers.Contains("Pagination"))
        {
            result.MetaData = JsonSerializer.Deserialize<PaginationModel>(response.Headers.GetValues("Pagination").First(), _options);
        }

        if (result.Success)
        {
            MemberCacheModel cacheResponse = new MemberCacheModel
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

    public async Task<ServiceResponseModel<string>> UpdateMemberAsync(MemberUpdateModel memberUpdate)
    {
        string apiEndpoint = _config["apiLocation"] + _config["membersEndpoint"];
        using HttpResponseMessage response = await _httpClient.PutAsJsonAsync(apiEndpoint, memberUpdate);
        ServiceResponseModel<string> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<string>>(_options);

        if (result.Success)
        {
            MemberModel member = (await GetMemberAsync(memberUpdate.Username)).Data;
            _mapper.Map(memberUpdate, member);
        }

        return result;
    }
}
