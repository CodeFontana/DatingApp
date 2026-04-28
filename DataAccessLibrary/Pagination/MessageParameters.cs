namespace DataAccessLibrary.Pagination;

public class MessageParameters : PaginationParameters
{
    public string Username { get; set; } = string.Empty;
    public string Container { get; set; } = string.Empty;
}
