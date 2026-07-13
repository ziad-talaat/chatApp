using Core.Common.newResultPattern;
using Core.Domain.Entities;
using Core.DTOS.PhotosDTOS;
using Core.Helper;
using Core.IServices;
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
    
    public class AdminController(UserManager<AppUser>_userManager,IPhotoService _photoService) : BaseApiController
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
            if (!roles.Split(',').Any(r => r == UserRoles.MemberRole))
            {
                return BadRequest(new Error("cant Delete Member Role"));
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

        [Authorize(Policy = AuthorizationPolicies.AdminModeratorPolicy)]
        [HttpPost("Approave-image")]
        public async Task< IActionResult> ApproaveImages(int imageId,bool isApproaved)
        {
            var userId = GetLoggedInUserId();
            if (userId == null)
                return BadRequest(new Error("user not exist log in first"));

            Result result = await _photoService.ApproaveImage(imageId, userId.Value, isApproaved);
            if (result.IsFailure)
                return BadRequest(result.Errors);

            return Ok();

        }

        [Authorize(Policy = AuthorizationPolicies.AdminModeratorPolicy)]
        [HttpGet("get-approvable-Images")]
        public async Task<ActionResult<List<PhotoDTO>>> GetApproavableImages(int imageId, bool isApproaved)
        {
            var result = await _photoService.GetApproavableImages();

            return Ok(result);

        }


    }
}
