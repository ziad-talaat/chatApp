using CloudinaryDotNet.Actions;
using Core.Common;
using Core.Domain.Entities;
using Core.DTOS.PhotosDTOS;
using Microsoft.AspNetCore.Http;
namespace Core.IServices
{
    public  interface IPhotoService
    {
        Task<(ResultResponse<ImageUploadResult> response, PhotoDTO? photo)> UploadPhotoAsync(IFormFile file, Guid id);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
        Task<bool> SetMainImage(int id, Guid userId);
        Task<bool> DeleteImage(int id, Guid userId);
        Task<bool> DisableMainImage(Guid userId);
    }
}
