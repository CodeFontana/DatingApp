using System.Collections.Generic;
using System.Threading.Tasks;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using DataAccessLibrary.Models;
using DataAccessLibrary.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(LogUserActivity))]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly IPhotoService _photoService;

        public UsersController(IUsersService usersService, IPhotoService photoService)
        {
            _usersService = usersService;
            _photoService = photoService;
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetUserAsync(string username)
        {
            ServiceResponseModel<MemberModel> response = await _usersService.GetUserAsync(username, User.Identity.Name);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersAsync([FromQuery] UserParameters userParameters)
        {
            PaginationResponseModel<PaginationList<MemberModel>> response = await _usersService.GetUsersAsync(User.Identity.Name, userParameters);

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

        [HttpGet("photo/{username}/{filename}")]
        public async Task<IActionResult> GetUserPhotoAsync(string username, string filename)
        {
            ServiceResponseModel<byte[]> response = await _photoService.GetPhotoAsync(username, filename);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserAsync(MemberUpdateModel memberUpdate)
        {
            ServiceResponseModel<string> response = await _usersService.UpdateUserAsync(User.Identity.Name, memberUpdate);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPost("photo/add")]
        public async Task<IActionResult> AddPhotoAsync([FromForm] IEnumerable<IFormFile> files)
        {
            ServiceResponseModel<PhotoModel> response = await _photoService.AddPhotoAsync(User.Identity.Name, files);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPut("photo/set-main")]
        public async Task<IActionResult> SetMainPhotoAsync([FromBody] int photoId)
        {
            ServiceResponseModel<string> response = await _photoService.SetMainPhotoAsync(User.Identity.Name, photoId);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPut("photo/delete")]
        public async Task<IActionResult> DeletePhotoAsync([FromBody] PhotoModel photo)
        {
            ServiceResponseModel<string> response = await _photoService.DeletePhotoAsync(User.Identity.Name, photo);

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
