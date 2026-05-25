namespace DatingApp.DataAccess.Internal;

public sealed class AccountUpdateData
{
    public int Id { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
