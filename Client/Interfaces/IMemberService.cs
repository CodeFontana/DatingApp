namespace Client.Interfaces;

public interface IMemberService
{
    List<MemberModel> MemberCache { get; set; }
    Dictionary<string, MemberCacheModel> MemberListCache { get; set; }
    UserParameters MembersFilter { get; set; }
    Task<ServiceResponseModel<MemberModel>> GetMemberAsync(string username);
    Task<PaginationResponseModel<IEnumerable<MemberModel>>> GetMembersAsync(UserParameters userParameters);
    Task<ServiceResponseModel<string>> UpdateMemberAsync(MemberUpdateModel memberUpdate);
}