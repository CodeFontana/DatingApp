using DatingApp.Client.Http;

namespace DatingApp.Client.Services;

public class MessageService : IMessageService, IAsyncDisposable
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;
    private readonly IPhotoService _photoService;
    private readonly IMemberStateService _memberStateService;
    private readonly JsonSerializerOptions _options;
    private HubConnection? _messageHub;

    public event Action? MessagesChanged;

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

    public List<MessageResponse> Messages { get; set; } = new();
    public bool ConnectedToHub => _messageHub?.State == HubConnectionState.Connected;

    public async Task ConnectAsync(string jwtToken, string otherUser)
    {
        if (_messageHub is not null)
        {
            return;
        }

        HubConnection hub = new HubConnectionBuilder()
            .WithUrl(_config["hubLocation"] + $"/message?user={otherUser}", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult<string?>(jwtToken);
            })
            .WithAutomaticReconnect()
            .Build();

        _messageHub = hub;

        hub.On<IEnumerable<MessageResponse>>("ReceiveMessageThread", async (messages) =>
        {
            Messages = messages.ToList();

            for (int i = 0; i < Messages.Count; i++)
            {
                Messages[i] = await ResolveUserPhoto(Messages[i]);
            }

            NotifyStateChanged();
            await hub.SendAsync("SendThreadAck", DateTime.UtcNow, otherUser);
        });

        hub.On<MessageResponse>("ReceiveMessage", async (message) =>
        {
            message = await ResolveUserPhoto(message);
            Messages = Messages.TakeLast(9).ToList();
            Messages.Add(message);
            NotifyStateChanged();
            await hub.SendAsync("SendThreadAck", DateTime.UtcNow, otherUser);
        });

        hub.On<DateTime>("ReceiveThreadAck", (ackTime) =>
        {
            foreach (MessageResponse m in Messages)
            {
                if (m.RecipientUsername == otherUser && m.DateRead == null)
                {
                    m.DateRead = ackTime;
                }
            }

            NotifyStateChanged();
        });

        await hub.StartAsync();
    }

    private void NotifyStateChanged() => MessagesChanged?.Invoke();

    public async Task DisconnectAsync()
    {
        if (_messageHub is not null)
        {
            await _messageHub.StopAsync();
            _messageHub = null;
        }
    }

    public async Task CreateHubMessageAsync(CreateMessageRequest messageCreateModel)
    {
        if (_messageHub is null)
        {
            throw new InvalidOperationException("Message hub is not connected.");
        }

        await _messageHub.SendAsync("SendMessage", messageCreateModel);
    }

    public async Task<ApiResponse<MessageResponse>> CreateMessageAsync(CreateMessageRequest messageCreateModel)
    {
        string apiEndpoint = _config["apiLocation"] + _config["messagesEndpoint"];
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(apiEndpoint, messageCreateModel);
        return await response.Content.ReadApiResponseAsync<MessageResponse>(_options);
    }

    public async Task<PaginatedResponse<List<MessageResponse>>> GetMessagesForMemberAsync(MessageListQuery messageParameters)
    {
        string apiEndpoint = _config["apiLocation"] + _config["messagesEndpoint"];

        Dictionary<string, string?> queryStringParam = new()
        {
            [nameof(messageParameters.PageNumber)] = messageParameters.PageNumber.ToString(),
            [nameof(messageParameters.PageSize)] = messageParameters.PageSize.ToString(),
            [nameof(messageParameters.Container)] = messageParameters.Container
        };

        using HttpResponseMessage response = await _httpClient.GetAsync(QueryHelpers.AddQueryString(apiEndpoint, queryStringParam));
        PaginatedResponse<List<MessageResponse>> result =
            await response.Content.ReadPaginatedResponseAsync<List<MessageResponse>>(_options);

        result.MetaData = response.Headers.ReadPaginationMetadata(_options);

        if (result.Success && result.Data is not null)
        {
            for (int i = 0; i < result.Data.Count; i++)
            {
                result.Data[i] = await ResolveUserPhoto(result.Data[i]);
            }
        }

        return result;
    }

    public async Task<ApiResponse<List<MessageResponse>>> GetMessageThreadAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentNullException(nameof(username), "Invalid username");
        }

        string apiEndpoint = _config["apiLocation"] + _config["messagesEndpoint"] + $"/thread/{username}";
        using HttpResponseMessage response = await _httpClient.GetAsync(apiEndpoint);
        ApiResponse<List<MessageResponse>> result =
            await response.Content.ReadApiResponseAsync<List<MessageResponse>>(_options);

        if (result.Success && result.Data is not null)
        {
            for (int i = 0; i < result.Data.Count; i++)
            {
                result.Data[i] = await ResolveUserPhoto(result.Data[i]);
            }
        }

        return result;
    }

    public async Task<ApiResponse<string>> DeleteMessageAsync(int id)
    {
        string apiEndpoint = _config["apiLocation"] + _config["messagesEndpoint"] + $"/{id}";
        using HttpResponseMessage response = await _httpClient.DeleteAsync(apiEndpoint);
        ApiResponse<string> result = await response.Content.ReadApiResponseAsync<string>(_options);
        Messages = new();
        return result;
    }

    private async Task<MessageResponse> ResolveUserPhoto(MessageResponse message)
    {
        string? currentUsername = _memberStateService.Member?.Username;

        if (currentUsername is not null && message.RecipientUsername == currentUsername)
        {
            message.RecipientPhotoUrl = _memberStateService.MainPhoto;
        }
        else
        {
            message.RecipientPhotoUrl = await _photoService.GetPhotoAsync(
                message.RecipientUsername, message.RecipientPhotoUrl);
        }

        if (currentUsername is not null && message.SenderUsername == currentUsername)
        {
            message.SenderPhotoUrl = _memberStateService.MainPhoto;
        }
        else
        {
            message.SenderPhotoUrl = await _photoService.GetPhotoAsync(
                message.SenderUsername, message.SenderPhotoUrl);
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
