namespace DataAccessLibrary.Interfaces;

public interface IMemberRepository
{
    void UpdateMember(AppUser user);
    Task<IEnumerable<AppUser>> GetMembersAsync();
    Task<AppUser> GetMemberByIdAsync(int id);
    Task<AppUser> GetMemberByUsernameAsync(string username);
    Task<PaginationList<MemberModel>> GetMembersAsync(MemberParameters userParameters);
    Task<MemberModel> GetMemberAsync(string username);
}
