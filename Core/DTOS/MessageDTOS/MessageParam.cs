using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common;
using Core.Domain.Enums;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Core.DTOS.MessageDTOS
{
    public class MessageParam<T>:GetPageResult<T>
    {
        public Guid MemberId { get; set; }
        public ContainerOptions Container { get; set; }= ContainerOptions.inbox;

    }
}
