namespace DatingApp.Client.Services;

public interface IMemberService
{
    List<MemberResponse> MemberCache { get; set; }
    Dictionary<string, MemberCacheResponse> MemberListCache { get; set; }
    Task<ApiResponse<MemberResponse>> GetMemberAsync(string username);
    Task<PaginatedResponse<IEnumerable<MemberResponse>>> GetMembersAsync(MemberListQuery userParameters);
    Task<ApiResponse<string>> UpdateMemberAsync(MemberUpdateRequest memberUpdate);
}