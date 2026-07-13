using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.DTOS.AuthDTOS;

namespace Core.IServices
{
    public interface IJWTService
    {
        Task<LoggedUserDTO> CreateJWTToken(AppUser appUserDTO);
        RefreshTokenDTO GetRefrehToken();
        Task<AppUser?> GetUserByValidRefreshToken(string refreshToken);
        ClaimsPrincipal? GetClaimsPrincipal(string token);
        Task LogOut();

    }
}
