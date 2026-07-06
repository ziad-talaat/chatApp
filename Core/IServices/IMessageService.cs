using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common;
using Core.Common.newResultPattern;
using Core.Domain.Entities;
using Core.DTOS.MessageDTOS;

namespace Core.IServices
{
    public  interface IMessageService
    {
        Task<Result<MessageDto>> AddMessage(CreateMessageDto messageDto,Guid currentUserId);
        Task<Result> DeleteMessage(Guid id, Guid memberId);
        Task<MessageDto?> GetMessage(Guid messageId);

        Task<GetPageResult<MessageDto>> GetMessagesForUser(MessageParam<MessageDto>messagesParams);

        Task<IReadOnlyList<MessageDto>> GetMessageThread(Guid currentUserId, Guid recipientId);
    }
}
