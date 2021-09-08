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
    [ApiController]
    [Authorize]
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
        public async Task<IActionResult> GetUsers()
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
        public async Task<IActionResult> GetUser(string username)
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
        public async Task<IActionResult> AddPhoto(IFormFile file)
        {
            ServiceResponseModel<PhotoModel> response = await _photoService.AddPhotoAsync(User.Identity.Name, file);

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
