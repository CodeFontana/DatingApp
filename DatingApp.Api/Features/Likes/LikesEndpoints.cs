using System.Security.Claims;
using DatingApp.Api.Infrastructure.Extensions;
using DatingApp.Api.Infrastructure.Filters;
using DatingApp.Contracts.Common;
using DatingApp.Contracts.Likes.Requests;
using DatingApp.Contracts.Members.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Api.Features.Likes;

public static class LikesEndpoints
{
    public static RouteGroupBuilder MapLikesEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/v1/likes")
            .WithTags("Likes")
            .RequireAuthorization()
            .AddEndpointFilter<UserActivityFilter>();

        group.MapGet("/", GetUserLikesAsync)
            .Produces<PaginatedResponse<IEnumerable<MemberResponse>>>();

        group.MapPost("/", ToggleLikeAsync)
            .Produces<ApiResponse<string>>();

        return group;
    }

    private static async Task<IResult> GetUserLikesAsync([AsParameters] LikesListQuery query, ILikesService likesService, HttpContext httpContext)
    {
        int userId = int.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        PaginatedResponse<IEnumerable<MemberResponse>> response = await likesService.GetUserLikesAsync(httpContext.User.Identity!.Name!, userId, query);
        return ApiResults.FromPaginated(response, httpContext);
    }

    private static async Task<IResult> ToggleLikeAsync([FromBody] string username, ILikesService likesService, HttpContext httpContext)
    {
        int sourceUserId = int.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        ApiResponse<string> response = await likesService.ToggleLikeAsync(httpContext.User.Identity!.Name!, username, sourceUserId);
        return ApiResults.FromResponse(response);
    }
}
