using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Domain.IRepository;
using Infrastructure.DataContext;
using Infrastructure.IRepository;

namespace Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
       
        public IRepository<AppUser> AppUser { get; private set; }
        public IRepository<Photo> PhotoRepository { get; private set; }
        public IRepository<UserLikes> UserLikesRepository { get; private set; }

        public int Complete()
        {
           return  _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }


        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            AppUser=new Repository<AppUser>(_context);
            PhotoRepository=new Repository<Photo>(_context);
            UserLikesRepository=new Repository<UserLikes>(_context);

        }
    }
}
