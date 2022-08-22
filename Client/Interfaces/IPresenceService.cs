namespace Client.Interfaces;

public interface IPresenceService
{
    Task ConnectAsync(string jwtToken);
    Task DisconnectAsync();
}