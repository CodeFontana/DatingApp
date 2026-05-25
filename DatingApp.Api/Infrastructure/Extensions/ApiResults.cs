using DatingApp.Contracts.Common;

namespace DatingApp.Api.Infrastructure.Extensions;

internal static class ApiResults
{
    public static IResult FromResponse<T>(ApiResponse<T> response, Func<ApiResponse<T>, IResult>? failure = null)
    {
        if (response.Success)
        {
            return Results.Ok(response);
        }

        return failure?.Invoke(response) ?? Results.BadRequest(response);
    }

    public static IResult FromPaginated<T>(PaginatedResponse<T> response, HttpContext httpContext)
    {
        if (response.Success)
        {
            httpContext.Response.AddPaginationHeader(response.MetaData);
            return Results.Ok(response);
        }

        return Results.BadRequest(response);
    }
}
