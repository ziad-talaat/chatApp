using System.Security.Claims;
using Core.IServices;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DatingApp.Filters.Actions
{
    public class LogUserActivity : Attribute,IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            var resultContext = await next();
            if (context.HttpContext.User.Identity?.IsAuthenticated != true)
            {
                return;
            }

            var status = Guid.TryParse(context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid id);
            if (!status) return;

            var memberService = resultContext.HttpContext.RequestServices.GetRequiredService<IAppUserService>();

            if (memberService==null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await memberService.LogLastActiveUser(id);

            

        }
    }
}
