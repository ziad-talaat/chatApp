using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common;
using Core.Domain.Enums;

namespace Core.DTOS.MemberDTOS
{
    public class MemberParams<T>:GetPageResult<T>
    {
        public string?  Gender { get; set; }
        public Guid? CurrentUserId{ get; set; }
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 100;
        public string OrderBy { get; set; } = nameof(orderingEnum.lastActive);

    }
}
