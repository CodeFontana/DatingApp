using DatingApp.Contracts.Admin.Responses;
using DatingApp.DataAccess.Internal;

namespace DatingApp.Api.Features.Admin;

internal static class AdminMapper
{
    public static UserWithRolesResponse ToResponse(UserWithRolesReadModel model) => new()
    {
        Id = model.Id,
        Username = model.Username,
        Roles = model.Roles
    };

    public static UserWithRolesReadModel ToReadModel(UserWithRolesResponse response) => new()
    {
        Id = response.Id,
        Username = response.Username,
        Roles = response.Roles
    };
}
