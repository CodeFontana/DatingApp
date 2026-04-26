namespace Client.Services;

public interface IPresenceService
{
    List<string> OnlineUsers { get; set; }

    event Action OnlineUsersChanged;
    event Action MessagesChanged;

    Task ConnectAsync(string jwtToken);
    Task DisconnectAsync();
}