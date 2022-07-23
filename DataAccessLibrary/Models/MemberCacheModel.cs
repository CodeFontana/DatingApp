namespace DataAccessLibrary.Models;

public class MemberCacheModel
{
    public DateTime CacheTime { get; set; }
    public string SearchKey { get; set; }
    public PaginationResponseModel<IEnumerable<MemberModel>> PaginatedResponse { get; set; }
}
