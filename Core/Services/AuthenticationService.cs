using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.DTOS.AuthDTOS;
using Core.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Core.Services
{
    public sealed class JWTService : IJWTService
    {
        private readonly IConfiguration _configuration;
        public JWTService(IConfiguration configuration)
        {
            _configuration= configuration;
        }
        public LoggedUserDTO CreateJWTToken(AppUser appUserDTO)
        {
            DateTime expirationDate = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JWT:expiration_Minutes"]));

            string issuer = _configuration["jwt:issuer"];
            string adiuence= _configuration["jwt:audience"];



            Claim[] claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,appUserDTO.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,  new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()),
                new Claim(ClaimTypes.NameIdentifier,appUserDTO.Email.ToString()),
                new Claim(ClaimTypes.Name,appUserDTO.UserName.ToString()),
            };

            SymmetricSecurityKey symmetricKey = new(Encoding.UTF8.GetBytes(_configuration["JWT:key"]));
            SigningCredentials credntials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken tokenGenerator = new JwtSecurityToken(
                issuer: issuer,
                audience: adiuence,
                claims:claims,
                expires: expirationDate,
                signingCredentials: credntials
                );

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
           var token= tokenHandler.WriteToken(tokenGenerator);



            return new LoggedUserDTO
            {
                Token = token,
                Email = appUserDTO.Email,
                UserName = appUserDTO.UserName,
                photoUrl = appUserDTO.ImageUrl,
                Id = appUserDTO.Id,
            };

        }

        public ClaimsPrincipal? GetClaimsPrincipal(string token)
        {
            var tokenValidation = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _configuration["jwt:issuer"],
                ValidAudience = _configuration["jwt:audience"],
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:key"]))
            };

            JwtSecurityTokenHandler jwtHandler = new();
            ClaimsPrincipal principles = jwtHandler.ValidateToken(token, tokenValidation, out SecurityToken validatedToken);

            if (validatedToken is not JwtSecurityToken jwtSecurityToken
                || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid Token");
            }
            return principles;
        }

        public RefreshTokenDTO GetRefrehToken()
        {
            return new RefreshTokenDTO
            {
                RefreshToken = GenerateRefreshToken(),
                RefreshTokenExpiration =  DateTime.UtcNow.Date.AddDays(Convert.ToDouble(_configuration["refreshToken:expiration_Date"]))
            };
        }

        private  string GenerateRefreshToken()
        {
            byte[] bytes= new byte[64];
            RandomNumberGenerator generator=  RandomNumberGenerator.Create();

            generator.GetBytes(bytes);
           return  Convert.ToBase64String(bytes);


        }
    }
}
