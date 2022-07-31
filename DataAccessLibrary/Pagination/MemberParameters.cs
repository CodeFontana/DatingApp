namespace DataAccessLibrary.Pagination;

public class MemberParameters : PaginationParameters
{
    public string CurrentUsername { get; set; }

    [MaxLength(25, ErrorMessage = "Invalid selection")]
    public string Gender { get; set; }

    [Range(18, 85, ErrorMessage ="Must be at least 18 years or older")]
    public int MinAge { get; set; } = 18;

    [Range(18, 85, ErrorMessage = "Sorry pops, you must be 85 or younger")]
    public int MaxAge { get; set; } = 45;

    public string OrderBy { get; set; } = "LastActive";

    public string Values
    {
        get 
        { 
            return $"MinAge({MinAge})-MaxAge({MaxAge})-Gender({Gender})-OrderBy({OrderBy})-PageSize({PageSize})-PageNumber({PageNumber})";
        }
    }
}
