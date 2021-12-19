using API.Helpers;
using API.Interfaces;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(LogUserActivity))]
    public class LikesController : ControllerBase
    {
        private readonly IUserLikeService _userLikeService;

        public LikesController(IUserLikeService userLikeService)
        {
            _userLikeService = userLikeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserLikesAsync([FromQuery] string predicate)
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
        public async Task<IActionResult> ToggleLikeAsync(string username)
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
}
