using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataContext
{
    public class Seed
    {

        public static async Task SeedUsers(UserManager<AppUser> _userManager ,RoleManager<IdentityRole<Guid>>_roleManager)
        {

            if (await _roleManager.Roles.AnyAsync()) return;

            var roles = new List<IdentityRole<Guid>>
               {
                   new IdentityRole<Guid>()
                   {
                       Id= Guid.NewGuid(),
                       Name=UserRoles.MemberRole,
                       NormalizedName= UserRoles.MemberRole.ToUpper(),
                   },
                   new IdentityRole<Guid>()
                   {
                       Id= Guid.NewGuid(),
                       Name=UserRoles.AdminRole,
                       NormalizedName= UserRoles.AdminRole.ToUpper(),
                   },
                   new IdentityRole<Guid>()
                   {
                       Id= Guid.NewGuid(),
                       Name=UserRoles.ModeratorRole,
                       NormalizedName= UserRoles.ModeratorRole.ToUpper(),
                   },
               };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role.Name!))
                {
                    await _roleManager.CreateAsync(role);
                }
            }


            if (await _userManager.Users.AnyAsync()) return;

            var owner = new AppUser()
            {
                UserName = "Admin",
                Email = "Admin@email.com",
                ImageUrl = null,
                Description = "owner of site",
                DateOfBirth = DateOnly.FromDateTime(new DateTime(2003, 5, 28)),
                Gender = "Male",
                City = "Fayoum",
                Country = "Egypt",
                LastActive = DateTime.UtcNow,
                Created = DateTime.UtcNow,
            };

                 var result= await _userManager.CreateAsync(owner, "12345_qweRT");

                if (!result.Succeeded)
                {
                  throw new InvalidDataException("Invaid User Data");
                }
              await  _userManager.AddToRolesAsync(owner, [UserRoles.AdminRole,UserRoles.ModeratorRole]);


           



        }
    }
}
