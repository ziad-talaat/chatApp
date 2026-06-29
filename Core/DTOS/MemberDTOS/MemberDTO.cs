using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.DTOS.MemberDTOS
{
    public  sealed class MemberDTO
    {
        public Guid Id { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? PhotoUrl { get; set; }
        public string UserName { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string Gender { get; set; }
        public string? Description { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

    }

}
