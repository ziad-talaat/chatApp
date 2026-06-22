using System.Security.Claims;
using Core.Domain.Entities;
using Core.DTOS.AuthDTOS;
using Core.DTOS.UserDTOS;
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
            //if (!ModelState.IsValid)
            //{
            //    var errors = GetErrors(ModelState);
            //    return BadRequest(errors);
            //}

            var user =await  _userManager.FindByEmailAsync(registerDto.Email);
            if (user != null)
                return BadRequest("Email is in use");

            user = new AppUser
            {
                Email = registerDto.Email,
                UserName= registerDto.DisplayName,

            };
             var result= await  _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                return BadRequest(result?.Errors.FirstOrDefault()?.Description);

            var response = _jwtService.CreateJWTToken(user);
            var refreshTokenDto = _jwtService.GetRefrehToken();
            user.RefreshToken= refreshTokenDto.RefreshToken;
            user.RefreshTokenExpiration= refreshTokenDto.RefreshTokenExpiration;
            Response.Cookies.Append(
             "refreshToken",
             refreshTokenDto.RefreshToken,
             new CookieOptions
             {
                 HttpOnly = true,
                 Secure = true,
                 SameSite = SameSiteMode.Strict,
                 Expires = refreshTokenDto.RefreshTokenExpiration
             });
            await _userManager.UpdateAsync(user);

            return Ok(response);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            //if (!ModelState.IsValid)
            //{
            //    var errors = GetErrors(ModelState);
            //    return BadRequest(errors);
            //}

            var user=await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return Unauthorized("Invalid Credintials");

           var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if(!result)
                return Unauthorized($"Invalid Credintials");

            var response = _jwtService.CreateJWTToken(user);
            var refreshTokenDto=_jwtService.GetRefrehToken();
            user.RefreshToken= refreshTokenDto.RefreshToken;
            user.RefreshTokenExpiration = refreshTokenDto.RefreshTokenExpiration;
            Response.Cookies.Append(
            "refreshToken",
            refreshTokenDto.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = refreshTokenDto.RefreshTokenExpiration
            });

            await _userManager.UpdateAsync(user);



            return Ok(response);

        }

        [HttpPost("generateNewAccessToken")]
        public async Task<IActionResult> GenerateNewAccessToken(string token)
        {
            if (token is null)
                return BadRequest("token is not exist");

            ClaimsPrincipal?principle=_jwtService.GetClaimsPrincipal(token);
            if (principle == null)
                return Unauthorized("invalid Token");

            string? email = principle.FindFirstValue(ClaimTypes.NameIdentifier);

            AppUser? user=await _userManager.FindByEmailAsync(email);

           var refreshToken = Request.Cookies["refreshToken"];
            if (refreshToken == null)
                return Unauthorized();


            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiration <= DateTime.UtcNow)
                return Unauthorized("invalid Data");

            var response = _jwtService.CreateJWTToken(user);

            var refreshTokens= _jwtService.GetRefrehToken();
            user.RefreshToken = refreshTokens.RefreshToken;
            user.RefreshTokenExpiration = refreshTokens.RefreshTokenExpiration;
            Response.Cookies.Append(
              "refreshToken",
              refreshTokens.RefreshToken,
              new CookieOptions
              {
                  HttpOnly = true,
                  Secure = true,
                  SameSite = SameSiteMode.Strict,
                  Expires = refreshTokens.RefreshTokenExpiration
              });
            
            await _userManager.UpdateAsync(user);
            return Ok(response);
        }

    }
}
