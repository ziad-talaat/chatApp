using System.Security.Claims;
using Core.Common;
using Core.Common.newResultPattern;
using Core.Domain.Entities;
using Core.DTOS.AuthDTOS;
using Core.DTOS.UserDTOS;
using Core.Helper;
using Core.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IJWTService _jwtService;
        private readonly UserManager<AppUser> _userManager;
        public AccountController(IJWTService jwtService, UserManager<AppUser> userManager)
        {
            _jwtService= jwtService;
            _userManager= userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTo registerDto)
        {
            var user =await  _userManager.FindByEmailAsync(registerDto.Email);
            if (user != null)
                return BadRequest("Email is in use");

            user = registerDto.ToAppUser();

            var result = await  _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                return BadRequest(result?.Errors.FirstOrDefault()?.Description);

            await _userManager.AddToRoleAsync(user, UserRoles.MemberRole);

            var response = await _jwtService.CreateJWTToken(user);
            var refreshTokenDto = _jwtService.GetRefrehToken();
            user.RefreshToken= refreshTokenDto.RefreshToken;
            user.RefreshTokenExpiration= refreshTokenDto.RefreshTokenExpiration;
            Response.Cookies.Append(
             "refreshToken",
             refreshTokenDto.RefreshToken,
             new CookieOptions
             {
                 HttpOnly = true,
                 Secure = false,
                 SameSite = SameSiteMode.Lax,
                 Expires = refreshTokenDto.RefreshTokenExpiration
             });
            await _userManager.UpdateAsync(user);

            return Ok(response);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return Unauthorized(new Error("Invalid Credintials"));

            if (await _userManager.IsLockedOutAsync(user))
                return Unauthorized(new Error("Account is locked"));



            if(! await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
               await _userManager.AccessFailedAsync(user);
                return Unauthorized(new Error($"Invalid Credintials"));
            }

          await _userManager.ResetAccessFailedCountAsync(user);

            var response = await _jwtService.CreateJWTToken(user);
            var refreshTokenDto = _jwtService.GetRefrehToken();
            user.RefreshToken = refreshTokenDto.RefreshToken;
            user.RefreshTokenExpiration = refreshTokenDto.RefreshTokenExpiration;
            Response.Cookies.Append(
            "refreshToken",
            refreshTokenDto.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = refreshTokenDto.RefreshTokenExpiration
            });

            await _userManager.UpdateAsync(user);

            return Ok(response);

        }

        [HttpPost("generateNewAccessToken")]
        public async Task<IActionResult> GenerateNewAccessToken()
        {
            //if (token is null)
            //    return BadRequest("token is not exist");



            ////ClaimsPrincipal?principle=_jwtService.GetClaimsPrincipal(token);
            ////if (principle == null)
            ////    return Unauthorized(new Error( "invalid Token"));

            ////string? id = principle.FindFirstValue(ClaimTypes.NameIdentifier);

            ////AppUser? user=await _userManager.FindByIdAsync(id);


           var refreshToken = Request.Cookies["refreshToken"];
            if (refreshToken == null)
                return Unauthorized();

            AppUser?user= await _jwtService.GetUserByValidRefreshToken(refreshToken);


            if (user == null)
                return Unauthorized(new Error("invalid Data"));


            var response =await _jwtService.CreateJWTToken(user);
            var refreshTokens= _jwtService.GetRefrehToken();
            user.RefreshToken = refreshTokens.RefreshToken;
            user.RefreshTokenExpiration = refreshTokens.RefreshTokenExpiration;
            Response.Cookies.Append(
              "refreshToken",
              refreshTokens.RefreshToken,
              new CookieOptions
              {
                  HttpOnly = true,
                  Secure = false,
                  SameSite = SameSiteMode.Lax,
                  Expires = refreshTokens.RefreshTokenExpiration
              });
            
            await _userManager.UpdateAsync(user);
            return Ok(response);
        }

    }
}
