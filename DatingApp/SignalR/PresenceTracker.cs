using System.Collections.Concurrent;

namespace DatingApp.SignalR
{
    public class PresenceTracker
    {
        private static readonly ConcurrentDictionary<Guid, 
            ConcurrentDictionary<string, byte>>OnLineUsers = new();


        public  Task UserConnected(Guid UserId,string connectionId)
        {
            var connections = OnLineUsers.GetOrAdd(UserId, _ => 
            new ConcurrentDictionary<string, byte>());

            connections.TryAdd(connectionId, 0);
            return Task.CompletedTask;

        }

        public Task UserDisconnected(Guid UserId,string connectionId)
        {
            if(OnLineUsers.TryGetValue(UserId, out var connections))
            {
                connections.TryRemove(connectionId,out _);
                if (connections.IsEmpty)
                {
                    OnLineUsers.TryRemove(UserId, out _);
                }
            }
           return  Task.CompletedTask;
        }


        public Task<Guid[]> GetOnLineUsers()
        {
            return Task.FromResult(OnLineUsers.Keys.OrderBy(x=>x).ToArray());
        }


        public static Task<List<string>> GetConcurrentForUser(Guid UserId)
        {
            if(OnLineUsers.TryGetValue(UserId,out var connections))
            {
                return Task.FromResult(connections.Keys.ToList());
            } 
            return Task.FromResult(new List<string>());
        }
    }
}
