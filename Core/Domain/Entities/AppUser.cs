using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities
{
    public class AppUser
    {
        public  string Id { get; set; } =Guid.NewGuid().ToString();
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

    }
}
