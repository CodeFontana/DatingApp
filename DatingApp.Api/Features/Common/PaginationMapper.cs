using DatingApp.Contracts.Common;

namespace DatingApp.Api.Features.Common;

internal static class PaginationMapper
{
    public static PaginationMetadata ToContract(DatingApp.DataAccess.Pagination.PaginationMetadata data) => new()
    {
        CurrentPage = data.CurrentPage,
        TotalPages = data.TotalPages,
        PageSize = data.PageSize,
        TotalCount = data.TotalCount
    };
}
