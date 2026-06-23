using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS.PhotosDTOS
{
    public sealed class PhotoDTO
    {
        public int PhotoId { get; set; }
        public string  PublicId { get; set; }
        public string  PhotoUrl { get; set; }
        public Guid  MemberId { get; set; }
    }
}
