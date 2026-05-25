using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.Api.Infrastructure.Hubs;

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
        string username = Context.User!.Identity!.Name!;
        bool isOnline = await _presenceTrackerService.UserConnected(username, Context.ConnectionId);

        if (isOnline)
        {
            await Clients.Others.SendAsync("UserIsOnline", username);
        }

        string[] currentUsers = await _presenceTrackerService.GetOnlineUsers();
        await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string username = Context.User!.Identity!.Name!;
        bool isOffline = await _presenceTrackerService.UserDisconnected(username, Context.ConnectionId);

        if (isOffline)
        {
            await Clients.Others.SendAsync("UserIsOffline", username);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
