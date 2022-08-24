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
        bool isOneline = await _presenceTrackerService.UserConnected(Context.User.Identity.Name, Context.ConnectionId);

        if (isOneline)
        {
            await Clients.Others.SendAsync("UserIsOnline", Context.User.Identity.Name);
        }
        
        string[] currentUsers = await _presenceTrackerService.GetOnlineUsers();
        await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        bool isOffline = await _presenceTrackerService.UserDisconnected(Context.User.Identity.Name, Context.ConnectionId);

        if (isOffline)
        {
            await Clients.Others.SendAsync("UserIsOffline", Context.User.Identity.Name);
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}
