using Core.Common.newResultPattern;
using Core.Domain.Entities;
using Core.Domain.IRepository;
using Core.DTOS.MessageDTOS;
using Core.Helper;
using Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;

namespace DatingApp.SignalR
{
    [Authorize]
    public class MessageHub(IMessageService _messageService ,IUnitOfWork _unitOfWork,IHubContext<PresenceHub>_presenceHub) :Hub
    {
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUserExist = string.IsNullOrEmpty(httpContext?.Request?.Query["userId"].ToString()) ? throw new HubException("cant find user") :
                              Guid.TryParse(httpContext?.Request?.Query["userId"].ToString(),out Guid result);
            if (!otherUserExist)
                throw new HubException("not valid user id");




            var groupName = GetGroupName(Context.User?.GetMemberId(), result);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

              await AddToGroup(groupName);
            var unReadMessagesForThisChat =await  _messageService.UnReadMessgaesForSpecificSenderUser(GetUserId(), result);
            var messages = await _messageService.GetMessageThread(GetUserId(), result);

            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);



            if(unReadMessagesForThisChat.Count > 0)
            {
                var connections =await PresenceTracker.GetConcurrentForUser(GetUserId());

                await _presenceHub.Clients.Clients(connections).SendAsync("updateUnReadMessages", unReadMessagesForThisChat);
            }


        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await _messageService.RemoveConnection(Context.ConnectionId);
            await  base.OnDisconnectedAsync(exception);
        }


        public async Task SendMessage(CreateMessageDto messageDto)
        {
            var senderId = GetUserId();

            var groupName = GetGroupName(senderId, messageDto.RecipientId);
            var group = await _messageService.GetMessageGroup(groupName);

            Result<MessageDto> messageDTOResult;

            var userInGroup = group != null && group.Connections.Any(x => x.UserId == messageDto.RecipientId);
            if (userInGroup)
            {
             messageDTOResult = await _messageService.AddMessage(messageDto, senderId,true);
            }
            else
            {
                messageDTOResult = await _messageService.AddMessage(messageDto, senderId);
            }
              
            if (messageDTOResult.IsSuccess) 
            {
                await Clients.Group(groupName).SendAsync("NewMessage", messageDTOResult.Value);
                var conectios = await PresenceTracker.GetConcurrentForUser(messageDto.RecipientId);
                if (conectios!=null && conectios.Count()>0 && !userInGroup)
                {
                    await _presenceHub.Clients.Clients(conectios).SendAsync("newUnreadMessage", messageDTOResult.Value);
                    await _presenceHub.Clients.Clients(conectios)
                        .SendAsync("MessageRecieved", messageDTOResult.Value);
                } 
            }
          
        }

        private static string GetGroupName(Guid? caller, Guid ?other)
        {
            var stringCompare= string.CompareOrdinal(caller.ToString(), other.ToString()) < 0 ;
            return stringCompare ? $"{caller}_{other}" : $"{other}_{caller}";
        }

        private Guid GetUserId()
        {
            return Context.User?.GetMemberId()??throw new HubException("cannot find the user");
        }

        private async Task AddToGroup(string groupName)
        {
            var group = await _messageService.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId,GetUserId());
            if (group == null)
            {
                 group=new Group(groupName);
                _messageService.AddGroup(group);
            }
             group.Connections.Add(connection);
            _unitOfWork.Complete();
             await Task.CompletedTask;
        }


    }
}
