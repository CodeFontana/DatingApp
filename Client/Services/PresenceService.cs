namespace Client.Services;

public class PresenceService : IAsyncDisposable, IPresenceService
{
    private readonly IConfiguration _config;
    private readonly ISnackbar _snackbar;
    private readonly IMessageService _messageService;
    private readonly NavigationManager _navman;
    private HubConnection _presenceHub;

    public event Action OnlineUsersChanged;
    public event Action MessagesChanged;

    public List<string> OnlineUsers { get; set; } = new();

    public PresenceService(IConfiguration config,
                           ISnackbar snackbar,
                           IMessageService messageService,
                           NavigationManager navman)
    {
        _config = config;
        _snackbar = snackbar;
        _messageService = messageService;
        _navman = navman;
    }

    public async Task ConnectAsync(string jwtToken)
    {
        if (_presenceHub == null)
        {
            _presenceHub = new HubConnectionBuilder()
            .WithUrl(_config["hubLocation"] + "/presence", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(jwtToken);
            })
            .WithAutomaticReconnect()
            .Build();

            _presenceHub.On<string>("UserIsOnline", (username) =>
            {
                OnlineUsers.Add(username);
                NotifyStateChanged();
            });

            _presenceHub.On<string>("UserIsOffline", (username) =>
            {
                OnlineUsers.Remove(username);
                NotifyStateChanged();
            });

            _presenceHub.On<string>("MessageReceived", (username) =>
            {
                if (_messageService.ConnectedToHub == false)
                {
                    NotifyMessagesChanged();

                    _snackbar.Add($"New Message from {username}!", Severity.Info, config =>
                    {
                        config.Onclick = snackbar =>
                        {
                            _navman.NavigateTo($"/member/{username}/messages");
                            return Task.CompletedTask;
                        };
                    });
                }
            });

            _presenceHub.On<string[]>("GetOnlineUsers", (usernames) =>
            {
                OnlineUsers = usernames.ToList();
                NotifyStateChanged();
            });

            await _presenceHub.StartAsync();
        }
    }

    public async Task DisconnectAsync()
    {
        await _presenceHub.StopAsync();
        await _presenceHub.DisposeAsync();
        OnlineUsers = new();
        _presenceHub = null;
    }

    private void NotifyStateChanged() => OnlineUsersChanged?.Invoke();
    private void NotifyMessagesChanged() => MessagesChanged?.Invoke();

    public async ValueTask DisposeAsync()
    {
        if (_presenceHub is not null)
        {
            await _presenceHub.DisposeAsync();
        }
    }
}
