namespace DataAccessLibrary.Pagination;

public class LikesParameters : PaginationParameters
{
    public int UserId { get; set; }
    public string Predicate { get; set; }

    public string Values
    {
        get
        {
            return $"User({UserId})-Predicate({Predicate})-PageSize({PageSize})-PageNumber({PageNumber})";
        }
    }
}
