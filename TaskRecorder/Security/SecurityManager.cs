using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskRecorder_DataModels.Models;

namespace TaskRecorder.Security
{
    public class SecurityManager
    {
        public async void SignIn(HttpContext httpContext, Users users, string schema)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(getUserClaims(users), schema);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await httpContext.SignInAsync(schema, claimsPrincipal);
        }
        public IEnumerable<Claim> getUserClaims(Users users)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, users.Email));
            foreach (var role in users.UsersRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Roles.RoleName));
            }
            return claims;
        }
        public async void SignOut(HttpContext httpContext, string schema)
        {
            await httpContext.SignOutAsync(schema);
        }
    }
}
