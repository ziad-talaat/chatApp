using Core.Common;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.Domain.IRepository;
using Core.DTOS.MemberDTOS;
using Core.DTOS.UserLikeDTOS;
using Core.Helper;
using Core.IServices;
using Microsoft.EntityFrameworkCore;
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
        //public async Task<GetPageResult<Guid>> GetCurrentMemberLikeIds(MemberParams<MemberDTO> memParams)
        //{
        //    var query =  _unitOfWork.UserLikesRepository.GetQuery
        //         .AsNoTracking().Where(x => x.SourceUserId == memParams.CurrentUserId)
        //         .Select(x => x.TargetUserId);

        //    return await GetPageResult<Guid>.GetPageAsync(query, memParams.CurrentPage, memParams.pageSize);

        //}
        public async Task<IReadOnlyList<Guid>> GetCurrentMemberLikeIds(Guid memberId)
        {
            return await _unitOfWork.UserLikesRepository.GetQuery
                 .AsNoTracking().Where(x => x.SourceUserId == memberId)
                 .Select(x => x.TargetUserId).ToListAsync();
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
        public async Task<MemberParams<MemberDTO>>  GetMemberLikes(LikeTypes likeTypes, MemberParams<MemberDTO> memParams)
        {
            var query = _unitOfWork.UserLikesRepository.GetQuery.Include(x=>x.SourceUser).Include(x=>x.TargetUser).AsNoTracking();
            List<MemberDTO>list = new List<MemberDTO>();
            if (likeTypes == LikeTypes.like)
            {
                query = query.Where(x => x.SourceUserId == memParams.CurrentUserId);
            }
            else if (likeTypes == LikeTypes.likedBy)
            {
               query= query.Where(x => x.TargetUserId == memParams.CurrentUserId);
            }
            else if(likeTypes == LikeTypes.mutual)
            {
                var likesId = (await GetCurrentMemberLikeIds(memParams.CurrentUserId.Value));
                query= query.Where(x => x.TargetUserId == memParams.CurrentUserId && likesId.Contains(x.SourceUserId));

            }
            var data=  await  GetPageResult<UserLikes>.GetPageAsync(query, memParams.CurrentPage, memParams.pageSize);


            if (likeTypes == LikeTypes.like)
            {
                list = data.PageResult.Select(x => x.TargetUser).Select(x => x.ToMemberDTO()).ToList();
            }
            else 
            {
                list = data.PageResult.Select(x => x.SourceUser).Select(x => x.ToMemberDTO()).ToList();
            }
            


              return  new MemberParams<MemberDTO>
            {
                CurrentPage = data.CurrentPage,
                pageSize = data.pageSize,
                TotalItems = data.TotalItems,
                PageResult = list
            };

        }
    }
}