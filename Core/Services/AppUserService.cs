using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Domain.IRepository;
using Core.DTOS.MemberDTOS;
using Core.IServices;
using Microsoft.EntityFrameworkCore;
using Core.Helper;
using Core.Common;
using Core.DTOS.PhotosDTOS;
using System.Security.Cryptography.X509Certificates;
namespace Core.Services
{
    public class AppUserService(IUnitOfWork unitOfWork) : IAppUserService
    {
        private readonly IUnitOfWork _unitOfWork=unitOfWork;
        public async Task<MemberDTO?> GetMemberById(Guid id)
        {
            var user =await  _unitOfWork.AppUser.GetById(id,false);
            if (user == null)
                return null;

           return user.ToMemberDTO();
        }


        public async Task<List<PhotoDTO>>GetMemberPhotos(Guid id)
        {
          return   await _unitOfWork.PhotoRepository.GetQuery.AsNoTracking()
                .Where(x=>x.UserId==id).Select(x=>x.ToPhotoDTO()).ToListAsync();
        }

        public async Task<List<MemberDTO>> GetMembers()
        {
            return await _unitOfWork.AppUser.GetQuery.AsNoTracking().Select(x => new MemberDTO
            {
                Id = x.Id,
                UserName=x.UserName,
                DateOfBirth=x.DateOfBirth,
                Gender=x.Gender,
                City=x.City,
                Country=x.Country,
                Description=x.Description,
                PhotoUrl=x.ImageUrl,
                Created=x.Created,
                LastActive=x.LastActive


            }).ToListAsync();
        }

        public async Task<ResultResponse<string>> UpdateMember(Guid id,EditMemberDTO memberDTO)
        {
            var user = await _unitOfWork.AppUser.GetById(id);
            if (user is null)
            {
                return new ResultResponse<string>
                {
                    Success = false,
                    DataSet = "No such User"
                };
            }

            user.City=memberDTO.City;
            user.Country=memberDTO.Country;
            user.Description=memberDTO.Description;
            user.UserName=memberDTO.UserName;

            _unitOfWork.AppUser.Update(user);
            _unitOfWork.Complete();

            return new ResultResponse<string>
            {
                Success = true,
                DataSet = "user updated successfully"
            };
        }
    }
}