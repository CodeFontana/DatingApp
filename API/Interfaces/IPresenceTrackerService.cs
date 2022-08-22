namespace API.Interfaces;

public interface IPresenceTrackerService
{
    Task<string[]> GetOnlineUsers();
    Task UserConnected(string username, string connectionId);
    Task UserDisconnected(string username, string connectionId);
}