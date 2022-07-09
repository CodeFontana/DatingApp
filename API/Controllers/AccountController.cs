using System.Threading.Tasks;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API.Interfaces;
using API.Filters;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [ServiceFilter(typeof(UserActivity))]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync(RegisterUserModel registerUser)
        {
            ServiceResponseModel<AuthUserModel> response = await _accountService.RegisterAsync(registerUser);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync(LoginUserModel loginUser)
        {
            ServiceResponseModel<AuthUserModel> response = await _accountService.LoginAsync(loginUser);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return Unauthorized(response);
            }
        }
    }
}
