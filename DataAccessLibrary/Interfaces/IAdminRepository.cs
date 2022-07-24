namespace DataAccessLibrary.Interfaces;

public interface IAdminRepository
{
    Task<IList<string>> EditRolesAsync(string username, string roles);
    Task<List<UserWithRolesModel>> GetUsersWithRolesAsync();
}