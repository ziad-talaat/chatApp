using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.IServices
{
    public interface IAppUserService
    {
        Task<List<AppUser>> GetMembers();
        Task<AppUser?> GetMemberById(Guid id);
    }
}
