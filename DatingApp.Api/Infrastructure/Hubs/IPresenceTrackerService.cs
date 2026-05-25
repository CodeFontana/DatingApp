namespace DatingApp.Api.Infrastructure.Hubs;

public interface IPresenceTrackerService
{
    Task<bool> UserConnected(string username, string connectionId);
    Task<bool> UserDisconnected(string username, string connectionId);
    Task<string[]> GetOnlineUsers();
}
