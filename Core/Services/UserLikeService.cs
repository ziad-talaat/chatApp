using System.Runtime.CompilerServices;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.Domain.IRepository;
using Core.DTOS.MemberDTOS;
using Core.DTOS.UserLikeDTOS;
using Core.Helper;
using Core.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
namespace Core.Services
{
    public sealed class UserLikeService : IUserLikesService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserLikeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }
        public int AddLike(UserLikeDto like)
        {
            UserLikes newLike = new UserLikes()
            {
                SourceUserId = like.SourceUserId,
                TargetUserId = like.TargetUserId,
            };
            _unitOfWork.UserLikesRepository.Insert(newLike);
           return  _unitOfWork.Complete();
        }
        public async Task<int>  DeleteLike(UserLikeDto like)
        {
            var newLike=await _unitOfWork.UserLikesRepository.GetQuery.FirstOrDefaultAsync(x=>x.SourceUserId == like.SourceUserId &&
            x.TargetUserId==like.TargetUserId);
            if (newLike == null)
                return 0;

            _unitOfWork.UserLikesRepository.Delete(newLike);
          return  _unitOfWork.Complete();

        }
        public async Task<IReadOnlyList<Guid>> GetCurrentMemberLikeIds(Guid memberId)
        {
           return   await _unitOfWork.UserLikesRepository.GetQuery
                .AsNoTracking().Where(x=>x.SourceUserId == memberId)
                .Select(x=>x.TargetUserId).ToListAsync();
        }
        public async  Task<UserLikeDto?> GetMemberLike(Guid sourceMemberId, Guid targetMemberId)
        {
            var like= await _unitOfWork.UserLikesRepository.GetQuery.FirstOrDefaultAsync(x => x.TargetUserId == targetMemberId && x.SourceUserId == sourceMemberId);
            if(like==null) return null;
            return new UserLikeDto
            {
                SourceUserId = like.SourceUserId,
                TargetUserId = like.TargetUserId,
            };
        }
        public async Task<IReadOnlyList<MemberDTO>> GetMemberLikes(LikeTypes likeTypes, Guid memberId)
        {
          var query=  _unitOfWork.UserLikesRepository.GetQuery.AsNoTracking();
            if (likeTypes == LikeTypes.like)
            {
                return await  query.Where(x=>x.SourceUserId==memberId)
                    .Select(x=>x.TargetUser).Select(x=>x.ToMemberDTO()).ToListAsync();
            }
            if(likeTypes == LikeTypes.likedBy)
            {
                return await query.Where(x => x.TargetUserId == memberId)
                   .Select(x => x.SourceUser).Select(x => x.ToMemberDTO()).ToListAsync();
            }
            var likesId=await GetCurrentMemberLikeIds(memberId);
            return await query.Where(x => x.TargetUserId == memberId && likesId.Contains(x.SourceUserId))
                .Select(x => x.SourceUser).Select(x => x.ToMemberDTO()).ToListAsync();
        }
    }
}