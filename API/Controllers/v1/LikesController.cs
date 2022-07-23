namespace API.Controllers.v1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
[ServiceFilter(typeof(UserActivity))]
public class LikesController : ControllerBase
{
    private readonly IUserLikeService _userLikeService;

    public LikesController(IUserLikeService userLikeService)
    {
        _userLikeService = userLikeService;
    }

    [HttpGet]
    [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<ActionResult<ServiceResponseModel<IEnumerable<LikeUserModel>>>> GetUserLikesAsync([FromQuery] string predicate)
    {
        int sourceUserId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        ServiceResponseModel<IEnumerable<LikeUserModel>> response = await _userLikeService.GetUserLikesAsync(User.Identity.Name, predicate, sourceUserId);

        if (response.Success)
        {
            return Ok(response);
        }
        else
        {
            return BadRequest(response);
        }
    }

    [HttpPost("{username}")]
    public async Task<ActionResult<ServiceResponseModel<string>>> ToggleLikeAsync(string username)
    {
        int sourceUserId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        ServiceResponseModel<string> response = await _userLikeService.ToggleLikeAsync(User.Identity.Name, username, sourceUserId);

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
