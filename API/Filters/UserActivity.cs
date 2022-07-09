using DataAccessLibrary.Entities;
using DataAccessLibrary.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Filters
{
    public class UserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ActionExecutedContext resultContext = await next();

            if (resultContext == null || resultContext.HttpContext.User.Identity.IsAuthenticated == false)
            {
                return;
            }

            int userId = int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            IUserRepository repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();
            AppUser user = await repo.GetUserByIdAsync(userId);
            user.LastActive = System.DateTime.Now;
            await repo.SaveAllAsync();
        }
    }
}
