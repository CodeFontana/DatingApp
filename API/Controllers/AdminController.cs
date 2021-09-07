using API.Interfaces;
using API.Services;
using DataAccessLibrary.Entities;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<IActionResult> GetUsersWithRoles()
        {
            ServiceResponseModel<List<UserWithRolesModel>> response = await _adminService.GetUsersWithRoles();

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
        public async Task<IActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            ServiceResponseModel<IList<string>> response = await _adminService.EditRoles(username, roles);

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
