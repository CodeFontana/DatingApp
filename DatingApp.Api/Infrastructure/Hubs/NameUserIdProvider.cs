using Microsoft.AspNetCore.SignalR;

namespace DatingApp.Api.Infrastructure.Hubs;

public class NameUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.Identity?.Name;
    }
}
