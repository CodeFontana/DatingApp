namespace API.Controllers.v1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("users-with-roles")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<ActionResult<PaginationResponseModel<PaginationList<UserWithRolesModel>>>> GetUsersWithRolesAsync([FromQuery] PaginationParameters pageParameters)
    {
        PaginationResponseModel<PaginationList<UserWithRolesModel>> response = await _adminService.GetUsersWithRolesAsync(User.Identity.Name, pageParameters);

        if (response.Success)
        {
            Response.AddPaginationHeader(response.MetaData);
            return Ok(response);
        }
        else
        {
            return BadRequest(response);
        }
    }

    [HttpPost("edit-roles/{username}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<ActionResult<ServiceResponseModel<IList<string>>>> EditRolesAsync(string username, [FromQuery] string roles)
    {
        ServiceResponseModel<IList<string>> response = await _adminService.EditRolesAsync(User.Identity.Name, username, roles);

        if (response.Success)
        {
            return Ok(response);
        }
        else
        {
            return BadRequest(response);
        }
    }

    [HttpGet("photos-to-moderate")]
    [Authorize(Policy = "ModeratePhotoRole")]
    public ActionResult<ServiceResponseModel<string>> GetPhotosForModeration()
    {
        ServiceResponseModel<string> response = _adminService.GetPhotosForModeration(User.Identity.Name);

        if (response.Success)
        {
            return Ok(response);
        }
        else
        {
            return BadRequest(response);
        }
    }
}
