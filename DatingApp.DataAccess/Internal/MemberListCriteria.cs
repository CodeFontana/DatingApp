namespace DatingApp.DataAccess.Internal;

public sealed class MemberListCriteria
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string CurrentUsername { get; init; } = string.Empty;
    public string Gender { get; init; } = string.Empty;
    public int MinAge { get; init; } = 18;
    public int MaxAge { get; init; } = 45;
    public string OrderBy { get; init; } = "LastActive";
}
