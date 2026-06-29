using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.CustomeValidation;
using Core.Domain.Entities;
using Core.DTOS.MemberDTOS;
using Core.DTOS.UserDTOS;

namespace Core.Helper
{
    public static class UserHelper
    {
        public static MemberDTO ToMemberDTO(this AppUser user)
        {
            return new MemberDTO
            {
                Id = user.Id,
                DateOfBirth = user.DateOfBirth,
                PhotoUrl = user?.ImageUrl,
                City = user.City,
                Country = user.Country,
                Description = user?.Description,
                Gender = user.Gender,
                LastActive = user.LastActive,
                Created = user.Created,
                UserName = user.UserName
            };
        }

        public static AppUser ToAppUser(this RegisterDTo user)
        {


            return new AppUser
            {
               
                DateOfBirth = user.DateOfBirth,
                City = user.City,
                Country = user.Country,
                Gender = user.Gender,
                Created = user.Created,
                UserName = user.UserName,
                 PhoneNumber = user.PhoneNumber,
                 Email= user.Email,
            };
        }
    }
}
