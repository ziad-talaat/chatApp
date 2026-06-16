
using Core.Domain.Entities;
using Infrastructure.IRepository;

namespace Core.Domain.IRepository
{
    public interface IUnitOfWork:IDisposable
    {
         IRepository<AppUser> AppUser { get; }
        int Complete();
       

    }
}
