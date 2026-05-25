using DatingApp.DataAccess.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DatingApp.Api.Infrastructure.Filters;

public sealed class UserActivityFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        object? result = await next(context);

        if (context.HttpContext.User.Identity?.IsAuthenticated != true)
        {
            return result;
        }

        IUnitOfWork unitOfWork = context.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
        string? username = context.HttpContext.User.Identity.Name;

        if (string.IsNullOrWhiteSpace(username))
        {
            return result;
        }

        var user = await unitOfWork.AccountRepository.GetAccountAsync(username);
        if (user is not null)
        {
            user.LastActive = DateTime.UtcNow;
            await unitOfWork.CompleteAsync();
        }

        return result;
    }
}
