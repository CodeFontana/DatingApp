using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            ServiceResponseModel<IEnumerable<MemberModel>> response = await _usersService.GetUsers();

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
            ServiceResponseModel<MemberModel> response = await _usersService.GetUser(username);

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
            string username = User.Identity.Name;
            ServiceResponseModel<string> response = await _usersService.UpdateUser(username, memberUpdate);

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
