namespace DataAccessLibrary.Interfaces;

public interface IAdminRepository
{
    Task<IList<string>> EditRolesAsync(string username, string roles);
    Task<PaginationList<UserWithRolesModel>> GetUsersWithRolesAsync(PaginationParameters pageParameters);
}