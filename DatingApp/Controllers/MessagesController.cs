using Core.Common.newResultPattern;
using Core.DTOS.MessageDTOS;
using Core.IServices;
using Microsoft.AspNetCore.Mvc;
using Core.Helper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Core.Common;
using Core.DTOS.MemberDTOS;
using Microsoft.AspNetCore.OutputCaching;
namespace DatingApp.Controllers
{
    
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IMessageService _messageService;
        public MessagesController(IMessageService messageService)
        {
            _messageService= messageService;
        }
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto messageDto)
        {
            var senderId = GetLoggedInUserId();
            if (senderId == null)
                return Unauthorized(new Error("login first"));

            var messageDTO= await  _messageService.AddMessage(messageDto, senderId.Value);
            return Ok(messageDTO.Value);
        }


        [HttpGet]
        public async Task<ActionResult<GetPageResult<MessageDto>>> GetMessagesByContainer([FromQuery]MessageParam<MessageDto> messparams)
        {
            var senderId = GetLoggedInUserId();
            if (senderId == null)
                return Unauthorized(new Error("login first"));
            messparams.MemberId=senderId.Value;


            GetPageResult<MessageDto> messagesPage = await _messageService.GetMessagesForUser(messparams);

            return Ok(messagesPage);
        }

        [HttpPost("getMessagesThread")]
        public async Task<ActionResult<IReadOnlyList<MessageDto>>> GetMessagesThread(Guid recipientId)
        {
            var senderId = GetLoggedInUserId();
            if (senderId == null)
                return Unauthorized(new Error("login first"));

           var  messagesDto = await _messageService.GetMessageThread(senderId.Value, recipientId);
            return Ok(messagesDto);
        }
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var senderId = GetLoggedInUserId();
            if (senderId == null)
                return Unauthorized(new Error("login first"));

            Result result= await  _messageService.DeleteMessage(id,senderId.Value);

            if (result.IsFailure)
                return BadRequest(result.Errors);

            return NoContent();

        }



    }
}
