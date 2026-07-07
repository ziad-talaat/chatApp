using Core.Common.newResultPattern;
using Core.Domain.Entities;
using DatingApp.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace DatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(UserManager<AppUser>_userManager) : ControllerBase
    {
        [Authorize(Policy = AuthorizationPolicies.AdminPolicy)]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await _userManager.Users.ToListAsync();
            var userList = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new
                {
                    user.Id,
                    user.Email,
                    Roles = roles.ToList()
                });
            }
            return Ok(userList);
        }



        [Authorize(Policy = AuthorizationPolicies.AdminPolicy)]
        [HttpPost("edit-roles/{userId}")]
        public async Task<ActionResult<IList<string>>> EditRole(Guid userId,[FromQuery]string roles)
        {
            if (string.IsNullOrEmpty(roles))
            {
                return BadRequest(new Error("you must provide at least one role"));
            }


            var selectedRoels = roles.Split(",").ToArray();

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return BadRequest(new Error("no such error"));


            var userRoles=await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, selectedRoels.Except(userRoles));
            if (!result.Succeeded)
                return BadRequest(new Error("Failed to add roles"));

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoels));
            if(!result.Succeeded)
                return BadRequest(new Error("Failed to remove roles"));

            return Ok(await _userManager.GetRolesAsync(user));
        }
       
    }
}
