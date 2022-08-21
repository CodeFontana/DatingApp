namespace Client.Interfaces;

public interface IAdminService
{
    Task<PaginationResponseModel<IEnumerable<UserWithRolesModel>>> GetUsersWithRolesAsync(PaginationParameters pageParameters);
}