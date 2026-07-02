using System.Security.Claims;
using DatingApp.Filters.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DatingApp.Controllers
{
    [LogUserActivity]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        [NonAction]
        public static List<string> GetErrors(ModelStateDictionary modelState)
        {
            return modelState
                .Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
        }

        [NonAction]
        public Guid? GetLoggedInUserId()
        {
            var status = Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid id);
            if (status) 
                return id;
            return null;
        }

        
    }
}
