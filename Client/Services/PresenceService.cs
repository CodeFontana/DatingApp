namespace Client.Services;

public class PresenceService : IAsyncDisposable, IPresenceService
{
    private readonly IConfiguration _config;
    private readonly ISnackbar _snackbar;
    private HubConnection _presenceHub;

    public PresenceService(IConfiguration config,
                           ISnackbar snackbar)
    {
        _config = config;
        _snackbar = snackbar;
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
                _snackbar.Add($"{username} is online", Severity.Info);
            });

            _presenceHub.On<string>("UserIsOffline", (username) =>
            {
                _snackbar.Add($"{username} is offline", Severity.Warning);
            });

            await _presenceHub.StartAsync();
        }
    }

    public async Task DisconnectAsync()
    {
        await _presenceHub.StopAsync();
        await _presenceHub.DisposeAsync();
        _presenceHub = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_presenceHub is not null)
        {
            await _presenceHub.DisposeAsync();
        }
    }
}
