namespace DataAccessLibrary.Interfaces;

public interface IUserRepository
{
    void Update(AppUser user);
    Task<bool> SaveAllAsync();
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUser> GetUserByIdAsync(int id);
    Task<AppUser> GetUserByUsernameAsync(string username);
    Task<PaginationList<MemberModel>> GetMembersAsync(UserParameters userParameters);
    Task<MemberModel> GetMemberAsync(string username);
}
