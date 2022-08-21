namespace API.Interfaces;

public interface IAdminService
{
    Task<PaginationResponseModel<PaginationList<UserWithRolesModel>>> GetUsersWithRolesAsync(string requestor, PaginationParameters pageParameters);
    Task<ServiceResponseModel<IList<string>>> EditRolesAsync(string requestor, string username, string roles);
    ServiceResponseModel<string> GetPhotosForModeration(string requestor);
}