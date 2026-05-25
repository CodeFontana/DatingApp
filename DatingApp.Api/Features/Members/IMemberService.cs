using DatingApp.Contracts.Common;
using DatingApp.Contracts.Members.Requests;
using DatingApp.Contracts.Members.Responses;

namespace DatingApp.Api.Features.Members;

public interface IMemberService
{
    Task<ApiResponse<MemberResponse>> GetMemberAsync(string username, string requestor);
    Task<PaginatedResponse<IEnumerable<MemberResponse>>> GetMembersAsync(string requestor, MemberListQuery query);
    Task<ApiResponse<string>> UpdateMemberAsync(string username, MemberUpdateRequest memberUpdate);
}
