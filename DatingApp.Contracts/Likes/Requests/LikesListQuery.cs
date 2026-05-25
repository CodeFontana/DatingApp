using DatingApp.Contracts.Common.Pagination;

namespace DatingApp.Contracts.Likes.Requests;

public class LikesListQuery : PaginationQuery
{
    public string Predicate { get; set; } = string.Empty;

    public string Values =>
        $"Predicate({Predicate})-PageSize({PageSize})-PageNumber({PageNumber})";
}
