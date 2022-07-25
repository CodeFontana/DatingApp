using static System.Net.Mime.MediaTypeNames;
using System.Reflection;

namespace DataAccessLibrary.Pagination;

public class LikesParameters : PaginationParameters
{
    public int UserId { get; set; }
    public string Predicate { get; set; }

    public string Values
    {
        get
        {
            return $"{UserId}-{Predicate}-{PageNumber}";
        }
    }
}
