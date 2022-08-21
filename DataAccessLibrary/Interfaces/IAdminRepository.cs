namespace DataAccessLibrary.Interfaces;

public interface IAdminRepository
{
    Task EditRolesAsync(UserWithRolesModel userWithRoles);
    Task<PaginationList<UserWithRolesModel>> GetUsersWithRolesAsync(PaginationParameters pageParameters);
}