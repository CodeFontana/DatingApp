namespace DatingApp.DataAccess.Internal;

public sealed class MessageListCriteria
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string Username { get; init; } = string.Empty;
    public string Container { get; init; } = string.Empty;
}
