using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using DatingApp.Contracts.Common;

namespace DatingApp.Client.Http;

internal static class HttpContentExtensions
{
    public static async Task<ApiResponse<T>> ReadApiResponseAsync<T>(
        this HttpContent content,
        JsonSerializerOptions options)
    {
        return await content.ReadFromJsonAsync<ApiResponse<T>>(options)
            ?? new ApiResponse<T> { Success = false, Message = "Empty response from API" };
    }

    public static async Task<PaginatedResponse<T>> ReadPaginatedResponseAsync<T>(
        this HttpContent content,
        JsonSerializerOptions options)
    {
        return await content.ReadFromJsonAsync<PaginatedResponse<T>>(options)
            ?? new PaginatedResponse<T> { Success = false, Message = "Empty response from API" };
    }

    public static PaginationMetadata ReadPaginationMetadata(
        this HttpResponseHeaders headers,
        JsonSerializerOptions options)
    {
        if (!headers.Contains("Pagination"))
        {
            return new PaginationMetadata();
        }

        return JsonSerializer.Deserialize<PaginationMetadata>(
            headers.GetValues("Pagination").First(), options)
            ?? new PaginationMetadata();
    }
}
