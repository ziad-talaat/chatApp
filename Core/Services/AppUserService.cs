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
using Core.Domain.Enums;
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


        public async Task<List<PhotoDTO>>GetMemberPhotos(Guid id, Guid LoggedUserId)
        {
            if (id == LoggedUserId)
            {

            return  await _unitOfWork.PhotoRepository.GetQuery.AsNoTracking()
                .Where(x=>x.UserId==id).Select(x=>x.ToPhotoDTO()).ToListAsync();
            }
            return await _unitOfWork.PhotoRepository.GetQuery.AsNoTracking()
               .Where(x => x.UserId == id && x.IsApproaved).Select(x => x.ToPhotoDTO()).ToListAsync();

        }

        public async Task<GetPageResult<MemberDTO>> GetMembers(MemberParams<MemberDTO> memParams)
        {
            var query = _unitOfWork.AppUser.GetQuery.AsNoTracking().Select(x => new MemberDTO
            {
                Id = x.Id,
                UserName = x.UserName,
                DateOfBirth = x.DateOfBirth,
                Gender = x.Gender,
                City = x.City,
                Country = x.Country,
                Description = x.Description,
                PhotoUrl = x.ImageUrl,
                Created = x.Created,
                LastActive = x.LastActive
            });

            query = query.Where(x=> x.Id != memParams.CurrentUserId);
            if(memParams.Gender != null)
            {
            query = query.Where(x=>x.Gender==memParams.Gender);

            }

            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-memParams.MaxAge-1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-memParams.MinAge));

            query=query.Where(x=>x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);

            query = memParams.OrderBy switch
            {
                nameof(orderingEnum.created) => query.OrderByDescending(x => x.Created),
                _ => query.OrderByDescending(x => x.LastActive),
            };

            return await GetPageResult<MemberDTO>.GetPageAsync(query, memParams.CurrentPage, memParams.pageSize);

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


        public async Task LogLastActiveUser(Guid id)
        {
           await _unitOfWork.AppUser.GetQuery.Where(x => x.Id == id).ExecuteUpdateAsync(x => x.SetProperty(x => x.LastActive, DateTime.UtcNow));
        }
    }
}