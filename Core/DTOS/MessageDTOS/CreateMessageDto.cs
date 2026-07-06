using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS.MessageDTOS
{
    public sealed  class CreateMessageDto
    {
        [Required]
        public  string Content { get; set; }
        public required Guid RecipientId { get; set; }
    }
}
