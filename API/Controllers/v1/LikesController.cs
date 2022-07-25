﻿namespace API.Controllers.v1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
[ServiceFilter(typeof(UserActivity))]
public class LikesController : ControllerBase
{
    private readonly ILikesService _userLikeService;

    public LikesController(ILikesService userLikeService)
    {
        _userLikeService = userLikeService;
    }

    [HttpGet]
    [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<ActionResult<PaginationResponseModel<PaginationList<LikeUserModel>>>> GetUserLikesAsync([FromQuery] LikesParameters likesParameters)
    {
        likesParameters.UserId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        PaginationResponseModel<PaginationList<LikeUserModel>> response = await _userLikeService.GetUserLikesAsync(User.Identity.Name, likesParameters);

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

    [HttpPost]
    public async Task<ActionResult<ServiceResponseModel<string>>> ToggleLikeAsync([FromBody] string username)
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
