namespace API.Extensions;

public static class HttpExtensions
{
    public static void AddPaginationHeader(this HttpResponse response, PaginationModel paginationData)
    {
        response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationData));
        response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
    }
}
