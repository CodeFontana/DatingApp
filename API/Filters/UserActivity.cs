namespace API.Filters;

public class UserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        ActionExecutedContext resultContext = await next();

        if (resultContext == null || resultContext.HttpContext.User.Identity.IsAuthenticated == false)
        {
            return;
        }

        IAccountRepository repo = resultContext!.HttpContext.RequestServices.GetService<IAccountRepository>();
        AppUser user = await repo.GetAccountAsync(resultContext.HttpContext.User.Identity.Name);
        user.LastActive = DateTime.UtcNow;
        await repo.SaveAllAsync();
    }
}
