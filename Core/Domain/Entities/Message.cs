using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities
{
    public class Message
    {
        public Guid Id { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime DateSent { get; set; }=DateTime.UtcNow;

        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }


        public Guid SenderId { get; set; }
        public AppUser Sender { get; set; }

        public Guid RecipientId { get; set; }
        public AppUser Recipient { get; set; }

    }
}
