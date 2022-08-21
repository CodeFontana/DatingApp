namespace API.Interfaces;

public interface IAdminService
{
    Task<PaginationResponseModel<PaginationList<UserWithRolesModel>>> GetUsersWithRolesAsync(string requestor, PaginationParameters pageParameters);
    Task<ServiceResponseModel<string>> EditRolesAsync(string requestor, UserWithRolesModel userWithRoles);
    ServiceResponseModel<string> GetPhotosForModeration(string requestor);
}