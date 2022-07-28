namespace Client.Interfaces;

public interface IMemberService
{
    List<MemberModel> MemberCache { get; set; }
    Dictionary<string, MemberCacheModel> MemberListCache { get; set; }
    Task<ServiceResponseModel<MemberModel>> GetMemberAsync(string username);
    Task<PaginationResponseModel<IEnumerable<MemberModel>>> GetMembersAsync(MemberParameters userParameters);
    Task<ServiceResponseModel<string>> UpdateMemberAsync(MemberUpdateModel memberUpdate);
}