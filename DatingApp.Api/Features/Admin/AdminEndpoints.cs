using DatingApp.Api.Infrastructure.Extensions;
using DatingApp.Contracts.Admin.Responses;
using DatingApp.Contracts.Common;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Api.Features.Admin;

public static class AdminEndpoints
{
    public static RouteGroupBuilder MapAdminEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/v1/admin")
            .WithTags("Admin")
            .RequireAuthorization();

        group.MapGet("/roles", GetRolesAsync)
            .RequireAuthorization("RequireAdminRole")
            .Produces<ApiResponse<List<string>>>();

        group.MapGet("/users-with-roles", GetUsersWithRolesAsync)
            .RequireAuthorization("RequireAdminRole")
            .Produces<ApiResponse<List<UserWithRolesResponse>>>();

        group.MapPost("/edit-roles", EditRolesAsync)
            .RequireAuthorization("RequireAdminRole")
            .Produces<ApiResponse<string>>();

        group.MapGet("/photos-to-moderate", GetPhotosForModeration)
            .RequireAuthorization("ModeratePhotoRole")
            .Produces<ApiResponse<string>>();

        return group;
    }

    private static async Task<IResult> GetRolesAsync(IAdminService adminService, HttpContext httpContext)
    {
        ApiResponse<List<string>> response = await adminService.GetRolesAsync(httpContext.User.Identity!.Name!);
        return ApiResults.FromResponse(response);
    }

    private static async Task<IResult> GetUsersWithRolesAsync(IAdminService adminService, HttpContext httpContext)
    {
        ApiResponse<List<UserWithRolesResponse>> response = await adminService.GetUsersWithRolesAsync(httpContext.User.Identity!.Name!);
        return ApiResults.FromResponse(response);
    }

    private static async Task<IResult> EditRolesAsync([FromBody] UserWithRolesResponse userWithRoles, IAdminService adminService, HttpContext httpContext)
    {
        ApiResponse<string> response = await adminService.EditRolesAsync(httpContext.User.Identity!.Name!, userWithRoles);
        return ApiResults.FromResponse(response);
    }

    private static IResult GetPhotosForModeration(IAdminService adminService, HttpContext httpContext)
    {
        ApiResponse<string> response = adminService.GetPhotosForModeration(httpContext.User.Identity!.Name!);
        return ApiResults.FromResponse(response);
    }
}
