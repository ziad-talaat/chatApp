using Core.IServices;
using Microsoft.AspNetCore.Mvc;
using Core.DTOS.UserLikeDTOS;
using Core.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace DatingApp.Controllers
{
    [Authorize]   
    public class LikesController : BaseApiController
    {

        private readonly IUserLikesService _userLikeService;
        public LikesController(IUserLikesService userLikeService)
        {
            _userLikeService = userLikeService;
        }

        [HttpPost("{targetUserId}")]
        public async Task<IActionResult>ToggleLike(Guid targetUserId)
        {
            var currentUSerId = GetLoggedInUserId();
            if (currentUSerId == null || currentUSerId == Guid.Empty)
                return Unauthorized();

            if (currentUSerId == targetUserId)
                return BadRequest("can't like yourself");

            var existingLike =await _userLikeService.GetMemberLike(currentUSerId.Value, targetUserId);

            int effectedRows = 0;

            if(existingLike == null)
            {
                 effectedRows=  _userLikeService.AddLike(new UserLikeDto(currentUSerId.Value, targetUserId));
               
            }
            else
            {
                effectedRows = await _userLikeService.DeleteLike(new UserLikeDto(currentUSerId.Value, targetUserId));
            }
            if (effectedRows > 0)
                return NoContent();
            return BadRequest("operation failed");
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetCurrentUserLikeIds()
        {
            var currentUSerId = GetLoggedInUserId();
            if (currentUSerId == null || currentUSerId == Guid.Empty)
                return Unauthorized();
          var result= await _userLikeService.GetCurrentMemberLikeIds(currentUSerId.Value);
            return Ok(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetMemberLikes(LikeTypes likeType)
        {
            var currentUSerId = GetLoggedInUserId();
            if (currentUSerId == null || currentUSerId == Guid.Empty)
                return Unauthorized();
            var members = await _userLikeService.GetMemberLikes(likeType, currentUSerId.Value);
            return Ok(members);
        }

    }
}
