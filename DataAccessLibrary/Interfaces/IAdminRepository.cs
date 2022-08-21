namespace DataAccessLibrary.Interfaces;

public interface IAdminRepository
{
    Task<List<UserWithRolesModel>> GetUsersWithRolesAsync();
    Task EditRolesAsync(UserWithRolesModel userWithRoles);
}