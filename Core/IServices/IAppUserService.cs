using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common;
using Core.Domain.Entities;
using Core.DTOS.MemberDTOS;
using Core.DTOS.PhotosDTOS;

namespace Core.IServices
{
    public interface IAppUserService
    {
        Task<List<MemberDTO>> GetMembers();
        Task<MemberDTO?> GetMemberById(Guid id);
        Task<ResultResponse<string>> UpdateMember(Guid id,EditMemberDTO memberDTO);
        Task<List<PhotoDTO>> GetMemberPhotos(Guid id);
    }
}
