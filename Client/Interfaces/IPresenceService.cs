namespace Client.Interfaces;

public interface IPresenceService
{
    List<string> OnlineUsers { get; set; }

    event Action OnlineUsersChanged;

    Task ConnectAsync(string jwtToken);
    Task DisconnectAsync();
}