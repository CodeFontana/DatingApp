namespace Client.Services;

public class MessageService : IMessageService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options;

    public MessageService(IConfiguration config,
                          HttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<PaginationResponseModel<IEnumerable<MessageModel>>> GetMessagesForMemberAsync(MessageParameters messageParameters)
    {
        string apiEndpoint = _config["apiLocation"] + _config["messagesEndpoint"];

        Dictionary<string, string> queryStringParam = new()
        {
            [nameof(messageParameters.PageNumber)] = messageParameters.PageNumber.ToString(),
            [nameof(messageParameters.PageSize)] = messageParameters.PageSize.ToString(),
            [nameof(messageParameters.Container)] = messageParameters.Container
        };

        using HttpResponseMessage response = await _httpClient.GetAsync(QueryHelpers.AddQueryString(apiEndpoint, queryStringParam));
        PaginationResponseModel<IEnumerable<MessageModel>> result = await response.Content.ReadFromJsonAsync<PaginationResponseModel<IEnumerable<MessageModel>>>(_options);

        if (response.Headers != null && response.Headers.Contains("Pagination"))
        {
            result.MetaData = JsonSerializer.Deserialize<PaginationModel>(response.Headers.GetValues("Pagination").First(), _options);
        }

        return result;
    }

    public async Task<ServiceResponseModel<IEnumerable<MessageModel>>> GetMessageThreadAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentNullException(nameof(username), "Invalid username");
        }

        string apiEndpoint = _config["apiLocation"] + _config["messagesEndpoint"] + $"/thread/{username}"; ;
        using HttpResponseMessage response = await _httpClient.GetAsync(apiEndpoint);
        ServiceResponseModel<IEnumerable<MessageModel>> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<IEnumerable<MessageModel>>>(_options);
        return result;
    }

    public async Task<ServiceResponseModel<MessageModel>> CreateMessageAsync(MessageCreateModel messageCreateModel)
    {
        string apiEndpoint = _config["apiLocation"] + _config["messagesEndpoint"];
        using HttpResponseMessage response = await _httpClient.PutAsJsonAsync(apiEndpoint, messageCreateModel);
        ServiceResponseModel<MessageModel> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<MessageModel>>(_options);
        return result;
    }
}
