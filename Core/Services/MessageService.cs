using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common;
using Core.Common.newResultPattern;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.Domain.IRepository;
using Core.DTOS.MessageDTOS;
using Core.Helper;
using Core.IServices;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Core.Services
{
    public sealed class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _memoryCach;
        private readonly MessageChachManager _cachManager;

       

        public MessageService(IUnitOfWork unitOfWork, IMemoryCache memoryCach, MessageChachManager cachManager)
        {
            _unitOfWork= unitOfWork;
            _memoryCach= memoryCach;
            _cachManager = cachManager;
        }

        public void AddGroup(Group group)
        {
           _unitOfWork.GroupRepository.Insert(group);
          
        }

        public async Task<Result<MessageDto>> AddMessage(CreateMessageDto messageDto, Guid currentUserId,bool userInChat=false)
        {
            var recipient= await _unitOfWork.AppUser.GetById(messageDto.RecipientId);
            var sender= await _unitOfWork.AppUser.GetById(currentUserId);
            if(sender==null || recipient == null || recipient.Id == currentUserId || string.IsNullOrEmpty( messageDto.Content))
            {
                return Result<MessageDto>.Failure(new Error("Invalid Data"));
            }

            var message=messageDto.GetMessage(currentUserId);
            if (userInChat)
            {
                message.DateRead= DateTime.UtcNow;
            }

           _unitOfWork.MessageRepository.Insert(message);
            _unitOfWork.Complete();
            _memoryCach.Remove($"getMessageThread_{currentUserId}_${messageDto.RecipientId}");
            if (_cachManager.UserTokens.TryGetValue(currentUserId, out var token))
            {
                token.Cancel();
                token.Dispose();

                _cachManager.UserTokens[currentUserId] = new CancellationTokenSource();
            }
            MessageDto dto = message.ToMessageDto();

            dto.SenderName = sender?.UserName;
            dto.SenderImageUrl = sender?.ImageUrl;




            return Result<MessageDto>.Success(dto);
        }
        public async Task<Result> DeleteMessage(Guid id, Guid memberId)
        {
          Message? message=  await _unitOfWork.MessageRepository.GetById(id);
            if (message == null)
                return Result.Failure(new Error("no uch message"));

            if (message.SenderId != memberId && message.RecipientId != memberId)
                return Result.Failure(new Error("you cant delete this message"));



            if(message.SenderId== memberId) message.SenderDeleted = true;
            if(message.RecipientId== memberId) message.RecipientDeleted = true;



            if(message.RecipientDeleted==true && message.SenderDeleted==true)
            {
                _unitOfWork.MessageRepository.Delete(message);
            }
            _unitOfWork.Complete();

            var otherUser =
    message.SenderId == memberId
        ? message.RecipientId
        : message.SenderId;

            _memoryCach.Remove($"getMessageThread_{memberId}_${otherUser}");
            _memoryCach.Remove($"getMessageThread_{otherUser}_${memberId}");


            if(_cachManager.UserTokens.TryGetValue(memberId, out var token)){
                token.Cancel();
                token.Dispose();

                _cachManager.UserTokens[memberId] = new CancellationTokenSource();
            }


            return Result.Success();
        }

        public async  Task<Connection?> GetConnection(string connectionId)
        {
           return await _unitOfWork.ConnectionRepository.GetById(connectionId);
        }

        public async  Task<Group?> GetGroupForConnection(string connectionId)
        {
            return await _unitOfWork.GroupRepository.GetQuery
                .Include(x => x.Connections).Where(x => x.Connections.Any(x => x.ConnectionId == connectionId)).FirstOrDefaultAsync();
        }

        public async Task<MessageDto?> GetMessage(Guid messageId)
        {
          var message= await _unitOfWork.MessageRepository.GetQuery.AsNoTracking().Include(x=>x.Sender).Include(x=>x.Recipient).SingleOrDefaultAsync(x=>x.Id==messageId);
            if (message == null)
                return null;
            return MessageHelper.ToMessageDto(message);
        }
        public async Task<Group?> GetMessageGroup(string groupName)
        {
            return await _unitOfWork.GroupRepository
                 .GetQuery.Include(x => x.Connections)
                 .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<GetPageResult<MessageDto>> GetMessagesForUser(MessageParam<MessageDto> messagesParams)
        {
           return( await  _memoryCach.GetOrCreateAsync($"MessagesForUser_{messagesParams.MemberId}_{messagesParams.Container}_{messagesParams.CurrentPage}_{messagesParams.pageSize}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);
                var token = _cachManager.GetUserToken(messagesParams.MemberId);
                entry.AddExpirationToken(new CancellationChangeToken(token.Token));
                var query = _unitOfWork.MessageRepository.GetQuery.Include(x=>x.Sender).Include(x=>x.Recipient).AsNoTracking();
                if (messagesParams.Container == ContainerOptions.inbox)
                {
                    query = query.Where(x => x.RecipientId == messagesParams.MemberId && x.RecipientDeleted!=true).OrderByDescending(x => x.DateSent); 

                }
                else
                {
                    query = query.Where(x => x.SenderId == messagesParams.MemberId && x.SenderDeleted != true).OrderByDescending(x => x.DateSent);
                }

                var dataQuery=query.Select(MessageHelper.ToMessageDto());
                GetPageResult<MessageDto> result = await GetPageResult<MessageDto>.GetPageAsync(dataQuery, messagesParams.CurrentPage, messagesParams.pageSize);
                return result;
            }))!;
           
        }

        public async Task<IReadOnlyList<MessageDto>> GetMessageThread(Guid currentUserId, Guid recipientId)
        {
             await _unitOfWork.MessageRepository
                 .GetQuery.Where(x => x.RecipientId == currentUserId && x.SenderId == recipientId && x.DateRead == null).ExecuteUpdateAsync(x=>x.SetProperty(m=>m.DateRead, DateTime.UtcNow));

            var data=(await _memoryCach.GetOrCreateAsync($"getMessageThread_{currentUserId}_${recipientId}",async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
               var messages= await _unitOfWork.MessageRepository.GetQuery.Include(x=>x.Sender)
                .Include(x=>x.Recipient)
                .Where(x => (x.RecipientId == currentUserId && x.RecipientDeleted==false && x.SenderId == recipientId)
                || (x.RecipientId == recipientId && x.SenderId == currentUserId  && x.SenderDeleted==false))
                .OrderBy(x => x.DateSent).Select(MessageHelper.ToMessageDto()).ToListAsync();
            return messages;
               
            }))!;

            return data;
        }

        public async Task<Result> RemoveConnection(string connectionId)
        {
            var connection=await _unitOfWork.ConnectionRepository.GetById(connectionId);
            if (connection == null) return Result.Failure(new Error("no such connection"));

            _unitOfWork.ConnectionRepository.Delete(connection);
            _unitOfWork.Complete();
            return Result.Success();
        }
    }
}
