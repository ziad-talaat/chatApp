using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS.UserLikeDTOS
{
    public sealed class UserLikeDto
    {
        public UserLikeDto()
        {
            
        }
        public UserLikeDto(Guid source,Guid target)
        {
            SourceUserId= source;
            TargetUserId= target;
        }
        public Guid SourceUserId { get; set; }
        public Guid TargetUserId { get; set; }
    }
}
