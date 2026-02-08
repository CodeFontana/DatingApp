using System.Collections.Generic;
using System.Threading.Tasks;
using API.Interfaces;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("roles")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<ActionResult<ServiceResponseModel<List<string>>>> GetRolesAsync()
    {
        ServiceResponseModel<List<string>> response = await _adminService.GetRolesAsync(User.Identity.Name);

        if (response.Success)
        {
            return Ok(response);
        }
        else
        {
            return BadRequest(response);
        }
    }

    [HttpGet("users-with-roles")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<ActionResult<ServiceResponseModel<List<UserWithRolesModel>>>> GetUsersWithRolesAsync()
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

    [HttpPost("edit-roles")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<ActionResult<ServiceResponseModel<string>>> EditRolesAsync(string username, UserWithRolesModel userWithRoles)
    {
        ServiceResponseModel<string> response = await _adminService.EditRolesAsync(User.Identity.Name, userWithRoles);

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
