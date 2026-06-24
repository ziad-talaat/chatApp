using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS.MemberDTOS
{
    public sealed class EditMemberDTO
    {

        [Required]
        [MinLength(2)]
        public string UserName  { get; set; }

        [Required]
        [MinLength(2)]
        public string City { get; set; }
        [Required]
        [MinLength(2)]
        public string Country { get; set; }

        public string? Description { get; set; }
    }
}
