using LRA.Gateways.Client.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LRA.Gateways.Client.Attributes;

public class RequireActiveSubscriptionAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.User.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var mail = context.HttpContext.Items["Mail"]?.ToString();
        if (string.IsNullOrEmpty(mail))
        {
            context.Result = new BadRequestObjectResult("User email not found in context.");
            return;
        }

        var subscriptionsService = context.HttpContext.RequestServices.GetService<ISubscriptionsServiceClient>();
        if (subscriptionsService == null)
        {
            context.Result = new BadRequestResult();
            return;
        }

        try
        {
            var hasActiveSubscription = subscriptionsService.CheckSubscriptionsExists(mail, context.HttpContext.RequestAborted).GetAwaiter().GetResult();
            if (!hasActiveSubscription)
            {
                context.Result = new ForbidResult("JwtAuthentication");
                return;
            }
        }
        catch (Exception ex)
        {
            context.Result = new BadRequestResult();
            return;
        }
    }
}
