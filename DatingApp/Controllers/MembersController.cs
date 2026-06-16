using System.Threading.Tasks;
using Core.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController(IAppUserService appUserService) : ControllerBase
    {
        private readonly IAppUserService _appUserService=appUserService;
        [HttpGet]
        public async  Task<IActionResult> GetMembers()
        {
              var users= await _appUserService.GetMembers();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetMemberById(string id)
        {
            var member = _appUserService.GetMemberById(id);
            return Ok(member);
        }
    }
}
