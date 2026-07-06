using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class MessageChachManager
    {
        public ConcurrentDictionary<Guid, CancellationTokenSource> UserTokens { get; }
        = new();



        public CancellationTokenSource GetUserToken(Guid id)
        {
            return UserTokens.GetOrAdd(id, _ => new CancellationTokenSource());
        }
    }
}
