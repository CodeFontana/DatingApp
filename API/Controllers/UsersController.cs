using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;
using API.Services;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly IPhotoService _photoService;

        public UsersController(IUsersService usersService, IPhotoService photoService)
        {
            _usersService = usersService;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersAsync()
        {
            ServiceResponseModel<IEnumerable<MemberModel>> response = await _usersService.GetUsers(User.Identity.Name);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetUserAsync(string username)
        {
            ServiceResponseModel<MemberModel> response = await _usersService.GetUser(username, User.Identity.Name);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpGet("{username}/{filename}")]
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
        public async Task<IActionResult> UpdateUser(MemberUpdateModel memberUpdate)
        {
            ServiceResponseModel<string> response = await _usersService.UpdateUser(User.Identity.Name, memberUpdate);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPost("add-photo")]
        public async Task<IActionResult> AddPhoto([FromForm] IEnumerable<IFormFile> files)
        {
            var requestUrl = $"{Request.Scheme}://{Request.Host.Value}/";
            ServiceResponseModel<PhotoModel> response = await _photoService.AddPhotoAsync(requestUrl, User.Identity.Name, files);

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
