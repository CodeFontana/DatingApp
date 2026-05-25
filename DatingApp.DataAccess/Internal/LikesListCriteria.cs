namespace DatingApp.DataAccess.Internal;

public sealed class LikesListCriteria
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public int UserId { get; init; }
    public string Predicate { get; init; } = string.Empty;
}
