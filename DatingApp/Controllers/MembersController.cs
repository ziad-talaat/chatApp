using Core.DTOS.MemberDTOS;
using Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> GetMemberById(Guid id)
        {
            var member = await _appUserService.GetMemberById(id);
            if (member == null)
                return NotFound("No such Member for this id");
            return Ok(member);
        }

        [HttpGet("{id}/photos")]
        public async Task<IActionResult> GetMemberPhotos(Guid id)
        {
            var memberPhotos = await _appUserService.GetMemberPhotos(id);
            return Unauthorized();
            //return Ok(memberPhotos);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> EditMember(Guid id,[FromBody] EditMemberDTO memberDTO)
        {
           var dataResult= await _appUserService.UpdateMember(id, memberDTO);
            if (dataResult.Success) { 
             return Ok(dataResult);
            }
            else
            {
                return BadRequest(dataResult);
            }
        }
    }
}
