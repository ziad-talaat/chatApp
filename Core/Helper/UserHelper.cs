using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.DTOS.MemberDTOS;

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
                ImageUrl = user?.ImageUrl,
                City = user.City,
                Country = user.Country,
                Description = user?.Description,
                Gender = user.Gender,
                LastActive = user.LastActive,
                Created = user.Created,
                UserName = user.UserName
            };
        }
    }
}
