using System.Threading.Tasks;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API.Interfaces;
using API.Filters;

namespace API.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
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
        public async Task<ActionResult<ServiceResponseModel<AuthUserModel>>> RegisterAsync(RegisterUserModel registerUser)
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
        public async Task<ActionResult<ServiceResponseModel<AuthUserModel>>> LoginAsync(LoginUserModel loginUser)
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
