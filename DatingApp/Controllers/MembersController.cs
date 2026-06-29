using System.Security.Claims;
using Core.DTOS.MemberDTOS;
using Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DatingApp.Filters.Actions;
using Core.Common;
using Core.Domain.Entities;
using Core.DTOS.PhotosDTOS;
namespace DatingApp.Controllers
{
    [Authorize]
    public class MembersController(IAppUserService appUserService,
        IPhotoService photoService) : BaseApiController
    {
        private readonly IAppUserService _appUserService=appUserService;
        private readonly IPhotoService _photoService = photoService;
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
            return Ok(memberPhotos);
        }


        [HttpPut("{id}")]
        [EligbleUser]
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



        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>>UploadPhoto([FromForm] IFormFile file)
        {
            Guid? userId = GetLoggedInUserId();
            if (userId is null)
            {
                return Unauthorized(new ResultResponse<string>
                {
                    Success = false,
                    DataSet = "You need To be logged in to upload image"
                });
            }
                var result=  await _photoService.UploadPhotoAsync(file, userId.Value);

            if(!result.response.Success || result.photo is null)
            {
                return Problem("Failed to Upload The image");
            }
            return Ok( result.photo);
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<IActionResult> SetMainImage(int photoId)
        {
            Guid? userId = GetLoggedInUserId();
            if (userId is null)
            {
                return Unauthorized("login in first");
            }

          bool isSuccess= await  _photoService.SetMainImage(photoId, userId.Value);
            if (!isSuccess)
                return BadRequest("Failed To Upload the image");

            return NoContent();
        }

        [HttpDelete("remove-photo/{photoId}")]
        public async Task<IActionResult> RemoveImage(int photoId)
        {
            Guid? userId = GetLoggedInUserId();
            if (userId is null)
            {
                return Unauthorized("login in first");
            }

            bool isSuccess = await _photoService.DeleteImage(photoId, userId.Value);
            if (!isSuccess)
                return BadRequest("Failed To Upload the image");

            return NoContent();
        }

        [HttpPost("disaple-main-image")]
        public async Task<IActionResult>DisableMainImage()
        {
            Guid? userId = GetLoggedInUserId();
            if(userId is null)
                return Unauthorized("login in first");

           var isSuccess=  await _photoService.DisableMainImage(userId.Value);

            if (isSuccess)
                return NoContent();

            return Problem("Failed To Disabled Image");
        }

    }
}
