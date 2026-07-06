using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.DTOS.MessageDTOS
{
    public sealed  class MessageDto
    {
        public  Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime DateSent { get; set; } 

        public Guid SenderId { get; set; }
        public string SenderName { get; set; }
        public string? SenderImageUrl { get; set; }

        public Guid RecipientId { get; set; }

        public string RecipientName { get; set; }
        public string? RecipientImageUrl { get; set; }
    }
}
