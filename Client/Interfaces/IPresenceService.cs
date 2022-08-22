namespace Client.Interfaces;

public interface IPresenceService
{
    List<string> OnelineUsers { get; set; }

    event Action OnlineUsersChanged;

    Task ConnectAsync(string jwtToken);
    Task DisconnectAsync();
}