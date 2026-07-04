using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Core.Common;
using Core.Domain.Entities;
using Core.Domain.IRepository;
using Core.DTOS.PhotosDTOS;
using Core.Helper.ConfigurationSections;
using Core.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
namespace Core.Services
{
    public sealed class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        private readonly IUnitOfWork _unitOfWork;
        public PhotoService(IOptions<CloudinarySection> config, IUnitOfWork unitOfWork)
        {
            var account = new Account(config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret);
            _cloudinary = new Cloudinary(account);
            _unitOfWork = unitOfWork;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deletionParam = new DeletionParams(publicId);
            return await _cloudinary.DestroyAsync(deletionParam);
        }

        public async Task<(ResultResponse<ImageUploadResult> response, PhotoDTO? photo)> UploadPhotoAsync(IFormFile file, Guid id)
        {
            var uploadReult = new ImageUploadResult();
            try
            {


                if (file.Length > 0)
                {
                    await using var stream = file.OpenReadStream();

                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                        Folder = $"users_images/{id}"
                    };
                    uploadReult = await _cloudinary.UploadAsync(uploadParams);
                }
                if (uploadReult.Error != null)
                {
                    return (
                     new ResultResponse<ImageUploadResult>()
                     {
                         Success = false,
                         DataSet = uploadReult
                     }
                        ,
                        null
                    );
                }
                var user = await _unitOfWork.AppUser.GetQuery.Include(x => x.Photos)
                     .FirstOrDefaultAsync(x => x.Id == id);

                if (user == null)
                {
                    return (
                        new ResultResponse<ImageUploadResult>()
                        {
                            Success = false,
                            DataSet = new ImageUploadResult()
                        }
                        ,
                        null
                        );

                }

                Photo photo = new Photo
                {
                    Url = uploadReult.SecureUrl.AbsoluteUri,
                    PublicId = uploadReult.PublicId,
                    UserId = id,

                };
                if (user.ImageUrl == null)
                {
                    user.ImageUrl = photo.Url;
                }
                user?.Photos?.Add(photo);
                _unitOfWork.AppUser.Update(user);
                _unitOfWork.Complete();

                return (
                    new ResultResponse<ImageUploadResult>()
                    {
                        Success = true,
                        DataSet = uploadReult
                    }
                    ,
                    new PhotoDTO
                    {
                        MemberId = photo.UserId,
                        PhotoId = photo.Id,
                        PublicId = photo.PublicId,
                        PhotoUrl = photo.Url,
                    }
                    );
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(uploadReult.PublicId))
                {
                    await DeletePhotoAsync(uploadReult.PublicId);

                }
                throw;
            }

        }


        public async Task<bool> SetMainImage(int id, Guid userId)
        {
            var user = await _unitOfWork.AppUser.GetQuery.Include(x => x.Photos.Where(x => x.Id == id))
                .FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                return false;
            }

            var photo = user.Photos.SingleOrDefault(x => x.Id == id);
            if (photo == null)
                return false;

            if (user.ImageUrl == photo.Url)
                return false;

            user.ImageUrl = photo.Url;
            _unitOfWork.AppUser.Update(user);
            _unitOfWork.Complete();
            return true;
        }

        public async Task<bool> DeleteImage(int id, Guid userId)
        {
            var user = await _unitOfWork.AppUser.GetQuery.AsTracking().Include(x => x.Photos.Where(x => x.Id == id))
                .FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                return false;
            }

            var photo = user.Photos.SingleOrDefault(x => x.Id == id);
            if (photo == null)
                return false;


           var result= await DeletePhotoAsync(photo.PublicId);
            if(result.Error != null)
                return false;


            if (user.ImageUrl == photo.Url)
                user.ImageUrl = null;
            _unitOfWork.PhotoRepository.Delete(photo);
            _unitOfWork.Complete();
            return true;
        }



        public async Task<bool> DisableMainImage(Guid userId)
        {
            var effected =await  _unitOfWork.AppUser.GetQuery.Where(x => x.Id == userId).ExecuteUpdateAsync(x => x.SetProperty(x => x.ImageUrl,(string?)null));

            return effected > 0;
        }

    }
}
