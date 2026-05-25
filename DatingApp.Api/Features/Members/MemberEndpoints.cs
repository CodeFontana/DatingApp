using DatingApp.Api.Infrastructure.Extensions;
using DatingApp.Api.Infrastructure.Filters;
using DatingApp.Contracts.Common;
using DatingApp.Contracts.Members.Requests;
using DatingApp.Contracts.Members.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Api.Features.Members;

public static class MemberEndpoints
{
    public static RouteGroupBuilder MapMemberEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/v1/members")
            .WithTags("Members")
            .RequireAuthorization()
            .AddEndpointFilter<UserActivityFilter>();

        group.MapGet("/{username}", GetMemberAsync)
            .Produces<ApiResponse<MemberResponse>>();

        group.MapGet("/", GetMembersAsync)
            .CacheOutput(policy => policy.Expire(TimeSpan.FromSeconds(30)))
            .Produces<PaginatedResponse<IEnumerable<MemberResponse>>>();

        group.MapGet("/photo/{username}/{filename}", GetMemberPhotoAsync)
            .Produces<ApiResponse<byte[]>>();

        group.MapPut("/", UpdateMemberAsync)
            .Produces<ApiResponse<string>>();

        group.MapPost("/photo/add", AddPhotoAsync)
            .DisableAntiforgery()
            .Produces<ApiResponse<PhotoResponse>>();

        group.MapPut("/photo/set-main", SetMainPhotoAsync)
            .Produces<ApiResponse<string>>();

        group.MapPost("/photo/delete", DeletePhotoAsync)
            .Produces<ApiResponse<string>>();

        return group;
    }

    private static async Task<IResult> GetMemberAsync(string username, IMemberService memberService, HttpContext httpContext)
    {
        ApiResponse<MemberResponse> response = await memberService.GetMemberAsync(username, httpContext.User.Identity!.Name!);
        return ApiResults.FromResponse(response);
    }

    private static async Task<IResult> GetMembersAsync([AsParameters] MemberListQuery query, IMemberService memberService, HttpContext httpContext)
    {
        PaginatedResponse<IEnumerable<MemberResponse>> response = await memberService.GetMembersAsync(httpContext.User.Identity!.Name!, query);
        return ApiResults.FromPaginated(response, httpContext);
    }

    private static async Task<IResult> GetMemberPhotoAsync(string username, string filename, IPhotoService photoService)
    {
        ApiResponse<byte[]> response = await photoService.GetPhotoAsync(username, filename);
        return ApiResults.FromResponse(response);
    }

    private static async Task<IResult> UpdateMemberAsync([FromBody] MemberUpdateRequest memberUpdate, IMemberService memberService, HttpContext httpContext)
    {
        ApiResponse<string> response = await memberService.UpdateMemberAsync(httpContext.User.Identity!.Name!, memberUpdate);
        return ApiResults.FromResponse(response);
    }

    private static async Task<IResult> AddPhotoAsync(IFormFileCollection files, IPhotoService photoService, HttpContext httpContext)
    {
        ApiResponse<PhotoResponse> response = await photoService.AddPhotoAsync(httpContext.User.Identity!.Name!, files);
        return ApiResults.FromResponse(response);
    }

    private static async Task<IResult> SetMainPhotoAsync([FromBody] int photoId, IPhotoService photoService, HttpContext httpContext)
    {
        ApiResponse<string> response = await photoService.SetMainPhotoAsync(httpContext.User.Identity!.Name!, photoId);
        return ApiResults.FromResponse(response);
    }

    private static async Task<IResult> DeletePhotoAsync([FromBody] PhotoResponse photo, IPhotoService photoService, HttpContext httpContext)
    {
        ApiResponse<string> response = await photoService.DeletePhotoAsync(httpContext.User.Identity!.Name!, photo);
        return ApiResults.FromResponse(response);
    }
}
