using System;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Caching.Memory;

namespace Core.Services
{
    public sealed class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _memoryCach;
        public MessageService(IUnitOfWork unitOfWork, IMemoryCache memoryCach)
        {
            _unitOfWork= unitOfWork;
            _memoryCach= memoryCach;
        }
        public async Task<Result<MessageDto>> AddMessage(CreateMessageDto messageDto, Guid currentUserId)
        {
            var recipient= await _unitOfWork.AppUser.GetById(messageDto.RecipientId);
            var sender= await _unitOfWork.AppUser.GetById(currentUserId);
            if(sender==null || recipient == null || recipient.Id == currentUserId || string.IsNullOrEmpty( messageDto.Content))
            {
                return Result<MessageDto>.Failure(new Error("Invalid Data"));
            }

            var message=messageDto.GetMessage(currentUserId);

           _unitOfWork.MessageRepository.Insert(message);
            _unitOfWork.Complete();
            _memoryCach.Remove($"getMessageThread_{currentUserId}_${messageDto.RecipientId}");
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
            return Result.Success();
        }

        public async Task<MessageDto?> GetMessage(Guid messageId)
        {
          var message= await _unitOfWork.MessageRepository.GetQuery.AsNoTracking().Include(x=>x.Sender).Include(x=>x.Recipient).SingleOrDefaultAsync(x=>x.Id==messageId);
            if (message == null)
                return null;
            return MessageHelper.ToMessageDto(message);

        }

        public async Task<GetPageResult<MessageDto>> GetMessagesForUser(MessageParam<MessageDto> messagesParams)
        {
           return( await  _memoryCach.GetOrCreateAsync($"MessagesForUser_{messagesParams.MemberId}_{messagesParams.Container}_{messagesParams.CurrentPage}_{messagesParams.pageSize}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);
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
    }
}
