using Core.Domain.Entities;
using Core.DTOS.MemberDTOS;
using Core.DTOS.PhotosDTOS;

namespace Core.Helper
{
    public static class PhotoHelper
    {
        public static PhotoDTO ToPhotoDTO(this Photo photo)
        {
            return new PhotoDTO
            {
               PhotoId=photo.Id,
               PhotoUrl=photo.Url,
               PublicId=photo?.PublicId,
               MemberId=photo.UserId,
               IsApproaved=photo.IsApproaved
            };
        }
    }
}
