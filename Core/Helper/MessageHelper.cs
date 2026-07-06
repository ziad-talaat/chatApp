using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.DTOS.MessageDTOS;

namespace Core.Helper
{
    public  static class MessageHelper
    {
        public static MessageDto ToMessageDto(this Message message)
        {
            return new MessageDto
            {
                Id = message.Id,
                Content = message.Content,
                DateRead = message?.DateRead,
                DateSent = message.DateSent,
                SenderId = message.SenderId,
                SenderName = message?.Sender?.UserName!,
                SenderImageUrl = message?.Sender?.ImageUrl!,
                RecipientId = message.RecipientId,
                RecipientName = message?.Recipient?.UserName!,
                RecipientImageUrl = message?.Recipient?.ImageUrl!
            };
        }

        public static  Expression<Func<Message,MessageDto>> ToMessageDto()
        {
            return message=>  new MessageDto
            {
                Id = message.Id,
                Content = message.Content,
                DateRead = message.DateRead,
                DateSent = message.DateSent,
                SenderId = message.SenderId,
                SenderName = message.Sender.UserName,
                SenderImageUrl = message.Sender.ImageUrl,
                RecipientId = message.RecipientId,
                RecipientName = message.Recipient.UserName,
                RecipientImageUrl = message.Recipient.ImageUrl
            };
        }

        public static Message GetMessage(this CreateMessageDto messageDto, Guid currentUserId)
        {
            return new Message
            {
                Content = messageDto.Content,
                RecipientId = messageDto.RecipientId,
                SenderId = currentUserId,
            };
        }


    
    }
}
