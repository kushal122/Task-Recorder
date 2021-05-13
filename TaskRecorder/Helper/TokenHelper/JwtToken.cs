using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TaskRecorder.Helper.TokenHelper
{
    public class JwtToken
    {
        public static string GenerateToken(string userId, string taskId, string email)
        {
            var mySecret = "my45123own%%%Token0*&1237)(Generator*@@*&^";
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

            var myIssuer = "Rabin Raut";
            var myAudience = "Rabin Raut";

            var tokenHandeller = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name,userId),
                    new Claim(ClaimTypes.Email,email),
                    new Claim(ClaimTypes.GivenName,taskId)
                }),
                Expires = DateTime.UtcNow.AddDays(2),
                Issuer = myIssuer,
                Audience = myAudience,
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandeller.CreateToken(tokenDescriptor);
            return tokenHandeller.WriteToken(token);
        }

        public static bool ValidateCurrentToken(string token)
        {
            var mySecret = "my45123own%%%Token0*&1237)(Generator*@@*&^";
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

            var myIssuer = "Rabin Raut";
            var myAudience = "Rabin Raut";

            var tokenHandeller = new JwtSecurityTokenHandler();
            try
            {
                tokenHandeller.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidIssuer = myIssuer,
                    ValidAudience = myAudience,
                    IssuerSigningKey = mySecurityKey
                }, out SecurityToken validateToken);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
