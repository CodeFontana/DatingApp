using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataAccessLibrary.Entities;
using DataAccessLibrary.Interfaces;
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
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MemberModel>>> GetUsers()
        {
            return Ok(await _userRepository.GetMembersAsync());
        }

        [HttpGet("{username}")]
        [Authorize]
        public async Task<ActionResult<MemberModel>> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult> UpdateUser(MemberUpdateModel memberUpdate)
        {
            string username = User.Identity.Name;
            AppUser user = await _userRepository.GetUserByUsernameAsync(username);
            _mapper.Map(memberUpdate, user);
            _userRepository.Update(user);

            if (await _userRepository.SaveAllAsync())
            {
                return NoContent();
            }
            else
            {
                return BadRequest("Failed to update user");
            }
        }
    }
}
