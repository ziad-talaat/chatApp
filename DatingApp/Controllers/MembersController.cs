using Core.IServices;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers
{
    public class MembersController(IAppUserService appUserService) : BaseApiController
    {
        private readonly IAppUserService _appUserService=appUserService;
        [HttpGet]
        public async  Task<IActionResult> GetMembers()
        {
              var users= await _appUserService.GetMembers();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMemberById(string id)
        {
            var member = await _appUserService.GetMemberById(id);
            return Ok(member);
        }
    }
}
