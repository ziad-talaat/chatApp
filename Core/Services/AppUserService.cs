using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Domain.IRepository;
using Core.IServices;

namespace Core.Services
{
    public class AppUserService(IUnitOfWork unitOfWork) : IAppUserService
    {
        private readonly IUnitOfWork _unitOfWork=unitOfWork;
        public Task<AppUser?> GetMemberById(string id)
        {
           var user= _unitOfWork.AppUser.GetById(id);
            return user;
        }

        public async Task<List<AppUser>> GetMembers()
        {
             return  await  _unitOfWork.AppUser.GetAll();
        }
    }
}
