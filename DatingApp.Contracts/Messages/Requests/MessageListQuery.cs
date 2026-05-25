using DatingApp.Contracts.Common.Pagination;

namespace DatingApp.Contracts.Messages.Requests;

public class MessageListQuery : PaginationQuery
{
    public string Container { get; set; } = string.Empty;
}
