namespace DatingApp.DataAccess.Interfaces;

public interface IMemberRepository
{
    void UpdateMember(AppUser user);
    Task<IEnumerable<AppUser>> GetMembersAsync();
    Task<AppUser?> GetMemberByIdAsync(int id);
    Task<AppUser?> GetMemberByUsernameAsync(string username);
    Task<PaginationList<MemberReadModel>> GetMembersAsync(MemberListCriteria criteria);
    Task<MemberReadModel?> GetMemberAsync(string username);
}
