namespace DatingApp.Api.Infrastructure.Hubs;

public class PresenceTrackerService : IPresenceTrackerService
{
    private static readonly Dictionary<string, List<string>> OnlineUsers = new();

    public Task<bool> UserConnected(string username, string connectionId)
    {
        bool isOnline = false;

        lock (OnlineUsers)
        {
            if (OnlineUsers.ContainsKey(username))
            {
                OnlineUsers[username].Add(connectionId);
            }
            else
            {
                OnlineUsers.Add(username, [connectionId]);
                isOnline = true;
            }
        }

        return Task.FromResult(isOnline);
    }

    public Task<bool> UserDisconnected(string username, string connectionId)
    {
        bool isOffline = false;

        lock (OnlineUsers)
        {
            if (!OnlineUsers.TryGetValue(username, out List<string>? connections))
            {
                return Task.FromResult(false);
            }

            connections.Remove(connectionId);

            if (connections.Count == 0)
            {
                OnlineUsers.Remove(username);
                isOffline = true;
            }
        }

        return Task.FromResult(isOffline);
    }

    public Task<string[]> GetOnlineUsers()
    {
        lock (OnlineUsers)
        {
            return Task.FromResult(OnlineUsers.OrderBy(k => k.Key).Select(u => u.Key).ToArray());
        }
    }
}
