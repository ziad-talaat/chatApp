using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities
{
    public class Connection(string connectionId,Guid userId)
    {
        public string ConnectionId { get; set; } = connectionId;
        public Guid UserId { get; set; } = userId;
        public Group Group { get; set; } = null!;
    }
}
