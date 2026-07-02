
using Core.Domain.Entities;
using Infrastructure.IRepository;

namespace Core.Domain.IRepository
{
    public interface IUnitOfWork:IDisposable
    {
         IRepository<AppUser> AppUser { get; }
        IRepository<Photo> PhotoRepository { get;  }
        IRepository<UserLikes> UserLikesRepository { get;  }
        int Complete();
       

    }
}
