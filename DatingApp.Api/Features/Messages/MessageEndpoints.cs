using DatingApp.Api.Infrastructure.Extensions;
using DatingApp.Api.Infrastructure.Filters;
using DatingApp.Contracts.Common;
using DatingApp.Contracts.Messages.Requests;
using DatingApp.Contracts.Messages.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Api.Features.Messages;

public static class MessageEndpoints
{
    public static RouteGroupBuilder MapMessageEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/v1/messages")
            .WithTags("Messages")
            .RequireAuthorization()
            .AddEndpointFilter<UserActivityFilter>();

        group.MapGet("/", GetMessagesAsync)
            .Produces<PaginatedResponse<IEnumerable<MessageResponse>>>();

        group.MapGet("/thread/{username}", GetMessageThreadAsync)
            .Produces<ApiResponse<IEnumerable<MessageResponse>>>();

        group.MapPost("/", CreateMessageAsync)
            .Produces<ApiResponse<MessageResponse>>();

        group.MapDelete("/{id:int}", DeleteMessageAsync)
            .Produces<ApiResponse<string>>();

        return group;
    }

    private static async Task<IResult> GetMessagesAsync([AsParameters] MessageListQuery query, IMessageService messageService, HttpContext httpContext)
    {
        PaginatedResponse<IEnumerable<MessageResponse>> response = await messageService.GetMessagesForMemberAsync(httpContext.User.Identity!.Name!, query);
        return ApiResults.FromPaginated(response, httpContext);
    }

    private static async Task<IResult> GetMessageThreadAsync(string username, IMessageService messageService, HttpContext httpContext)
    {
        ApiResponse<IEnumerable<MessageResponse>> response = await messageService.GetMessageThreadAsync(httpContext.User.Identity!.Name!, username);
        return ApiResults.FromResponse(response);
    }

    private static async Task<IResult> CreateMessageAsync([FromBody] CreateMessageRequest message, IMessageService messageService, HttpContext httpContext)
    {
        ApiResponse<MessageResponse> response = await messageService.CreateMessageAsync(httpContext.User.Identity!.Name!, message);
        return ApiResults.FromResponse(response);
    }

    private static async Task<IResult> DeleteMessageAsync(int id, IMessageService messageService, HttpContext httpContext)
    {
        ApiResponse<string> response = await messageService.DeleteMessageAsync(httpContext.User.Identity!.Name!, id);
        return ApiResults.FromResponse(response);
    }
}
