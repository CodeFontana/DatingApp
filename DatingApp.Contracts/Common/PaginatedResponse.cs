namespace DatingApp.Contracts.Common;

public class PaginatedResponse<T> : ApiResponse<T>
{
    public PaginationMetadata MetaData { get; set; } = new();
}
