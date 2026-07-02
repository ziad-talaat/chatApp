using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.DTOS.MemberDTOS;
using Core.DTOS.UserLikeDTOS;

namespace Core.IServices
{
    public  interface IUserLikesService
    {
        Task<UserLikeDto?> GetMemberLike(Guid sourceMemberId, Guid targetMemberId);
        Task<IReadOnlyList<MemberDTO>> GetMemberLikes(LikeTypes likeTypes, Guid memberId);
        Task<IReadOnlyList<Guid>> GetCurrentMemberLikeIds(Guid memberId);
        Task<int>  DeleteLike(UserLikeDto like);
        int  AddLike(UserLikeDto like);
        
    }
}
