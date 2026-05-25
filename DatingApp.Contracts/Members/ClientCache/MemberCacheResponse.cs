using DatingApp.Contracts.Common;
using DatingApp.Contracts.Members.Responses;

namespace DatingApp.Contracts.Members.ClientCache;

public class MemberCacheResponse
{
    public DateTime CacheTime { get; set; }
    public string SearchKey { get; set; } = string.Empty;
    public PaginatedResponse<IEnumerable<MemberResponse>> PaginatedResponse { get; set; } = new();
}
