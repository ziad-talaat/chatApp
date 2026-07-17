using System.Security.Claims;
using Core.Helper;
using Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.SignalR
{
    [Authorize]
    public class PresenceHub(PresenceTracker _tracker,IMessageService _messageService):Hub
    {
        public override async  Task OnConnectedAsync()
        {
            await _tracker.UserConnected(GetUserId(), Context.ConnectionId);
             await Clients.Others.SendAsync("UserOnline",GetUserId());

            var currentUsers=await  _tracker.GetOnLineUsers();
             await Clients.All.SendAsync("GetOnLineUsers", currentUsers);

            var unreadMessages=await _messageService.UnReadMessgaes(GetUserId());

            await Clients.Caller.SendAsync("unReadMessage", unreadMessages);

            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
              await   _tracker.UserDisconnected(GetUserId(), Context.ConnectionId);
            await Clients.Others.SendAsync("UserOffline", GetUserId());

            var currentUsers = await _tracker.GetOnLineUsers();
            await Clients.All.SendAsync("GetOnLineUsers", currentUsers);


            await  base.OnDisconnectedAsync(exception);
        }

        private Guid GetUserId()
        {
            return Context.User?.GetMemberId()??throw new HubException("cant get use ID");
        }

       
    }

}
