namespace API.Interfaces;

public interface IPresenceTrackerService
{
    Task<string[]> GetOnlineUsers();
    Task<bool> UserConnected(string username, string connectionId);
    Task<bool> UserDisconnected(string username, string connectionId);
}