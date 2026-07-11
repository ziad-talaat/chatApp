
using Core.Domain.Entities;
using Infrastructure.IRepository;

namespace Core.Domain.IRepository
{
    public interface IUnitOfWork:IDisposable
    {
         IRepository<AppUser> AppUser { get; }
        IRepository<Photo> PhotoRepository { get;  }
        IRepository<UserLikes> UserLikesRepository { get;  }
        IRepository<Message> MessageRepository { get;  }
        IRepository<Group> GroupRepository { get;  }
        IRepository<Connection> ConnectionRepository { get;  }
        int Complete();
       

    }
}
