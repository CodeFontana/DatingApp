using API.Filters;
using API.Interfaces;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ServiceFilter(typeof(UserActivity))]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    // GET: api/v1/Accounts
    [HttpGet]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<ActionResult<ServiceResponseModel<List<AccountModel>>>> Get()
    {
        ServiceResponseModel<List<AccountModel>> response = await _accountService.GetAccountsAsync(HttpContext.User.Identity.Name);

        if (response.Success)
        {
            return Ok(response);
        }
        else
        {
            return BadRequest(response);
        }
    }

    // GET api/v1/Accounts/username
    [HttpGet("{username}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<ActionResult<ServiceResponseModel<AccountModel>>> Get(string username)
    {
        ServiceResponseModel<AccountModel> response = await _accountService.GetAccountAsync(HttpContext.User.Identity.Name, username);

        if (response.Success)
        {
            return Ok(response);
        }
        else
        {
            return BadRequest(response);
        }
    }

    // POST api/v1/Accounts
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResponseModel<AuthUserModel>>> Post([FromBody] RegisterUserModel registerUser)
    {
        ServiceResponseModel<AuthUserModel> response = await _accountService.RegisterAsync(HttpContext.User.Identity.Name, registerUser);

        if (response.Success)
        {
            return Ok(response);
        }
        else
        {
            return BadRequest(response);
        }
    }

    // POST api/v1/Accounts/login
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResponseModel<AuthUserModel>>> Login([FromBody] LoginUserModel loginUser)
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

    // PUT api/v1/Accounts
    [HttpPut]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<ActionResult<ServiceResponseModel<bool>>> Put([FromBody] AccountUpdateModel updateAccount)
    {
        ServiceResponseModel<bool> response = await _accountService.UpdateAccountAsync(HttpContext.User.Identity.Name, updateAccount);

        if (response.Success)
        {
            return Ok(response);
        }
        else
        {
            return Unauthorized(response);
        }
    }

    // DELETE api/v1/Accounts/username
    [HttpDelete("{username}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<ActionResult<ServiceResponseModel<bool>>> Delete(string username)
    {
        ServiceResponseModel<bool> response = await _accountService.DeleteAccountAsync(HttpContext.User.Identity.Name, username);

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
