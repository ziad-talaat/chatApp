using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Core.Common;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.DTOS.MemberDTOS;
using Core.DTOS.UserLikeDTOS;

namespace Core.IServices
{
    public  interface IUserLikesService
    {
        Task<UserLikeDto?> GetMemberLike(Guid sourceMemberId, Guid targetMemberId);
        Task<MemberParams<MemberDTO>> GetMemberLikes(LikeTypes likeTypes, MemberParams<MemberDTO> memParams);
        //Task<GetPageResult<Guid>> GetCurrentMemberLikeIds(MemberParams<MemberDTO> memParams);
        Task<IReadOnlyList<Guid>> GetCurrentMemberLikeIds(Guid memberId);
        Task<int>  DeleteLike(UserLikeDto like);
        int  AddLike(UserLikeDto like);
        
    }
}
