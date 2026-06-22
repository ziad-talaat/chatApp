using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Core.Domain.Entities
{
    public class AppUser:IdentityUser<Guid>
    {
        public string? RefreshToken { get; set; } 
        public DateTime? RefreshTokenExpiration { get; set; } 
        public DateOnly DateOfBirth { get; set; } 
        public string? ImageUrl  { get; set; } 
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string Gender { get; set; }
        public string? Description { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public ICollection<Photo>? Photos { get; set; } = new List<Photo>();

    }
}
