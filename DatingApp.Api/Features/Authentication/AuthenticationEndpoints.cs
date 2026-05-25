using DatingApp.Api.Infrastructure.Extensions;
using DatingApp.Api.Infrastructure.Filters;
using DatingApp.Contracts.Authentication.Requests;
using DatingApp.Contracts.Authentication.Responses;
using DatingApp.Contracts.Common;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Api.Features.Authentication;

public static class AuthenticationEndpoints
{
    public static RouteGroupBuilder MapAuthenticationEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/v1/accounts")
            .WithTags("Authentication")
            .AddEndpointFilter<UserActivityFilter>();

        group.MapGet("/", GetAccountsAsync)
            .RequireAuthorization("RequireAdminRole")
            .Produces<ApiResponse<List<AccountResponse>>>();

        group.MapGet("/{username}", GetAccountAsync)
            .RequireAuthorization("RequireAdminRole")
            .Produces<ApiResponse<AccountResponse>>();

        group.MapPost("/", RegisterAsync)
            .AllowAnonymous()
            .Produces<ApiResponse<AuthResponse>>();

        group.MapPost("/login", LoginAsync)
            .AllowAnonymous()
            .Produces<ApiResponse<AuthResponse>>();

        group.MapPut("/", UpdateAccountAsync)
            .RequireAuthorization("RequireAdminRole")
            .Produces<ApiResponse<bool>>();

        group.MapDelete("/{username}", DeleteAccountAsync)
            .RequireAuthorization("RequireAdminRole")
            .Produces<ApiResponse<bool>>();

        return group;
    }

    private static async Task<IResult> GetAccountsAsync(IAccountService accountService, HttpContext httpContext)
    {
        ApiResponse<List<AccountResponse>> response = await accountService.GetAccountsAsync(httpContext.User.Identity!.Name!);
        return ApiResults.FromResponse(response);
    }

    private static async Task<IResult> GetAccountAsync(string username, IAccountService accountService, HttpContext httpContext)
    {
        ApiResponse<AccountResponse> response = await accountService.GetAccountAsync(httpContext.User.Identity!.Name!, username);
        return ApiResults.FromResponse(response);
    }

    private static async Task<IResult> RegisterAsync([FromBody] RegisterRequest registerUser, IAccountService accountService, HttpContext httpContext)
    {
        ApiResponse<AuthResponse> response = await accountService.RegisterAsync(httpContext.User.Identity?.Name, registerUser);
        return ApiResults.FromResponse(response);
    }

    private static async Task<IResult> LoginAsync([FromBody] LoginRequest loginUser, IAccountService accountService)
    {
        ApiResponse<AuthResponse> response = await accountService.LoginAsync(loginUser);
        return ApiResults.FromResponse(response, r => Results.Json(r, statusCode: StatusCodes.Status401Unauthorized));
    }

    private static async Task<IResult> UpdateAccountAsync([FromBody] AccountUpdateRequest updateAccount, IAccountService accountService, HttpContext httpContext)
    {
        ApiResponse<bool> response = await accountService.UpdateAccountAsync(httpContext.User.Identity!.Name!, updateAccount);
        return ApiResults.FromResponse(response, r => Results.Json(r, statusCode: StatusCodes.Status401Unauthorized));
    }

    private static async Task<IResult> DeleteAccountAsync(string username, IAccountService accountService, HttpContext httpContext)
    {
        ApiResponse<bool> response = await accountService.DeleteAccountAsync(httpContext.User.Identity!.Name!, username);
        return ApiResults.FromResponse(response);
    }
}
