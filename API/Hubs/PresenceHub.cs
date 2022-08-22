namespace API.Hubs;

[Authorize]
public class PresenceHub : Hub
{
    private readonly IPresenceTrackerService _presenceTrackerService;

    public PresenceHub(IPresenceTrackerService presenceTrackerService)
    {
        _presenceTrackerService = presenceTrackerService;
    }
    
    public override async Task OnConnectedAsync()
    {
        await _presenceTrackerService.UserConnected(Context.User.Identity.Name, Context.ConnectionId);
        await Clients.Others.SendAsync("UserIsOnline", Context.User.Identity.Name);
        string[] currentUsers = await _presenceTrackerService.GetOnlineUsers();
        await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await _presenceTrackerService.UserDisconnected(Context.User.Identity.Name, Context.ConnectionId);
        await Clients.Others.SendAsync("UserIsOffline", Context.User.Identity.Name);

        string[] currentUsers = await _presenceTrackerService.GetOnlineUsers();
        await Clients.All.SendAsync("GetOnlineUsers", currentUsers);

        await base.OnDisconnectedAsync(exception);
    }
}
