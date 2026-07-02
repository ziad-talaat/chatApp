using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities
{
    public class UserLikes
    {
        public  Guid SourceUserId { get; set; }
        public AppUser SourceUser { get; set; } = null!;
        public  Guid TargetUserId { get; set; }
        public AppUser TargetUser { get; set; } = null!;
    }
}
