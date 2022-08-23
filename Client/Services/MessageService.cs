namespace Client.Services;

public class MessageService : IMessageService, IAsyncDisposable
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;
    private readonly IPhotoService _photoService;
    private readonly IMemberStateService _memberStateService;
    private readonly JsonSerializerOptions _options;
    private HubConnection _messageHub;

    public event Action MessagesChanged;

    public MessageService(IConfiguration config,
                          HttpClient httpClient,
                          IPhotoService photoService,
                          IMemberStateService memberStateService)
    {
        _config = config;
        _httpClient = httpClient;
        _photoService = photoService;
        _memberStateService = memberStateService;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public List<MessageModel> Messages { get; set; } = new();

    public async Task ConnectAsync(string jwtToken, string otherUser)
    {
        if (_messageHub == null)
        {
            _messageHub = new HubConnectionBuilder()
            .WithUrl(_config["hubLocation"] + $"/message?user={otherUser}", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(jwtToken);
            })
            .WithAutomaticReconnect()
            .Build();

            _messageHub.On<IEnumerable<MessageModel>>("ReceiveMessageThread", async (messages) =>
            {
                Messages = messages.ToList();

                for (int i = 0; i < messages.Count(); i++)
                {
                    Messages[i] = await ResolveUserPhoto(Messages[i]);
                }

                NotifyStateChanged();
            });

            _messageHub.On<MessageModel>("ReceiveMessage", async (message) =>
            {
                message = await ResolveUserPhoto(message);
                Messages.Add(message);
                NotifyStateChanged();
            });

            await _messageHub.StartAsync();
        }
    }

    private void NotifyStateChanged() => MessagesChanged?.Invoke();

    public async Task DisconnectAsync()
    {
        if (_messageHub != null)
        {
            await _messageHub.StopAsync();
            _messageHub = null;
        }
    }

    public async Task CreateHubMessageAsync(MessageCreateModel messageCreateModel)
    {
        await _messageHub.SendAsync("SendMessage", messageCreateModel);
    }

    public async Task<ServiceResponseModel<MessageModel>> CreateMessageAsync(MessageCreateModel messageCreateModel)
    {
        string apiEndpoint = _config["apiLocation"] + _config["messagesEndpoint"];
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(apiEndpoint, messageCreateModel);
        ServiceResponseModel<MessageModel> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<MessageModel>>(_options);
        return result;
    }

    public async Task<PaginationResponseModel<List<MessageModel>>> GetMessagesForMemberAsync(MessageParameters messageParameters)
    {
        string apiEndpoint = _config["apiLocation"] + _config["messagesEndpoint"];

        Dictionary<string, string> queryStringParam = new()
        {
            [nameof(messageParameters.PageNumber)] = messageParameters.PageNumber.ToString(),
            [nameof(messageParameters.PageSize)] = messageParameters.PageSize.ToString(),
            [nameof(messageParameters.Container)] = messageParameters.Container
        };

        using HttpResponseMessage response = await _httpClient.GetAsync(QueryHelpers.AddQueryString(apiEndpoint, queryStringParam));
        PaginationResponseModel<List<MessageModel>> result = await response.Content.ReadFromJsonAsync<PaginationResponseModel<List<MessageModel>>>(_options);

        if (response.Headers != null && response.Headers.Contains("Pagination"))
        {
            result.MetaData = JsonSerializer.Deserialize<PaginationModel>(response.Headers.GetValues("Pagination").First(), _options);
        }

        if (result.Success)
        {
            for (int i = 0; i < result.Data.Count; i++)
            {
                result.Data[i] = await ResolveUserPhoto(result.Data[i]);
            }
        }

        return result;
    }

    public async Task<ServiceResponseModel<List<MessageModel>>> GetMessageThreadAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentNullException(nameof(username), "Invalid username");
        }

        string apiEndpoint = _config["apiLocation"] + _config["messagesEndpoint"] + $"/thread/{username}"; ;
        using HttpResponseMessage response = await _httpClient.GetAsync(apiEndpoint);
        ServiceResponseModel<List<MessageModel>> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<List<MessageModel>>>(_options);

        if (result.Success)
        {
            for (int i = 0; i < result.Data.Count; i++)
            {
                result.Data[i] = await ResolveUserPhoto(result.Data[i]);
            }
        }

        return result;
    }

    public async Task<ServiceResponseModel<string>> DeleteMessageAsync(int id)
    {
        string apiEndpoint = _config["apiLocation"] + _config["messagesEndpoint"] + $"/{id}";
        using HttpResponseMessage response = await _httpClient.DeleteAsync(apiEndpoint);
        ServiceResponseModel<string> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<string>>(_options);
        Messages = new();
        return result;
    }

    private async Task<MessageModel> ResolveUserPhoto(MessageModel message)
    {
        if (message.RecipientUsername == _memberStateService.AppUser.Username)
        {
            message.RecipientPhotoUrl = _memberStateService.MainPhoto;
        }
        else
        {
            message.RecipientPhotoUrl = await _photoService.GetPhotoAsync(message.RecipientUsername, message.RecipientPhotoUrl);
        }

        if (message.SenderUsername == _memberStateService.AppUser.Username)
        {
            message.SenderPhotoUrl = _memberStateService.MainPhoto;
        }
        else
        {
            message.SenderPhotoUrl = await _photoService.GetPhotoAsync(message.SenderUsername, message.SenderPhotoUrl);
        }

        return message;
    }

    public async ValueTask DisposeAsync()
    {
        if (_messageHub is not null)
        {
            await _messageHub.DisposeAsync();
        }
    }
}
