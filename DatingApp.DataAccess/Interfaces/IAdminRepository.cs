namespace DatingApp.DataAccess.Interfaces;

public interface IAdminRepository
{
    Task<List<UserWithRolesReadModel>> GetUsersWithRolesAsync();
    Task EditRolesAsync(UserWithRolesReadModel userWithRoles);
    Task<List<string>> GetRolesAsync();
}
