using System.Text.Json;
using DatingApp.Contracts.Common;
using Microsoft.AspNetCore.Http;

namespace DatingApp.Api.Infrastructure.Extensions;

public static class HttpExtensions
{
    public static void AddPaginationHeader(this HttpResponse response, PaginationMetadata paginationData)
    {
        response.Headers.Append("Pagination", JsonSerializer.Serialize(paginationData));
        response.Headers.Append("Access-Control-Expose-Headers", "Pagination");
    }
}
