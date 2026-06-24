using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DatingApp.Filters.Actions
{
    public class EligbleUser : Attribute,IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userId=context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var routeId =context.RouteData.Values["id"]?.ToString();


           
            if (!Guid.TryParse(routeId, out Guid idGuid)||
            !Guid.TryParse(userId, out Guid userIDGuid))
            {
                context.Result= new  UnauthorizedResult();
                return;
            }


            if (idGuid != userIDGuid)
            {
                context.Result= new  ForbidResult();
                return;
            }

            await next();
        }
    }
}
