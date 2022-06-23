using API.Interfaces;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<IActionResult> GetUsersWithRolesAsync()
        {
            ServiceResponseModel<List<UserWithRolesModel>> response = await _adminService.GetUsersWithRolesAsync(User.Identity.Name);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPost("edit-roles/{username}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> EditRolesAsync(string username, [FromQuery] string roles)
        {
            ServiceResponseModel<IList<string>> response = await _adminService.EditRolesAsync(username, roles);

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
        public IActionResult GetPhotosForModeration()
        {
            ServiceResponseModel<string> response = _adminService.GetPhotosForModeration();

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
