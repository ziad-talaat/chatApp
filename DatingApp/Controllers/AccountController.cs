using Core.DTOS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers
{
    
    public class AccountController : BaseApiController
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(AppUserDTO user)
        {
            return Ok();
        }
    }
}
