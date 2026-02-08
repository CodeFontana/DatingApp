using System.Security.Claims;
using System.Threading.Tasks;
using API.Extensions;
using API.Filters;
using API.Interfaces;
using DataAccessLibrary.Models;
using DataAccessLibrary.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
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
    public async Task<ActionResult<PaginationResponseModel<PaginationList<MemberModel>>>> GetUserLikesAsync([FromQuery] LikesParameters likesParameters)
    {
        likesParameters.UserId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        PaginationResponseModel<PaginationList<MemberModel>> response = await _userLikeService.GetUserLikesAsync(User.Identity.Name, likesParameters);

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
