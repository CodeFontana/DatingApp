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

        IUnitOfWork unitOfWork = resultContext!.HttpContext.RequestServices.GetService<IUnitOfWork>();
        AppUser user = await unitOfWork.AccountRepository.GetAccountAsync(resultContext.HttpContext.User.Identity.Name);
        user.LastActive = DateTime.UtcNow;
        await unitOfWork.Complete();
    }
}
