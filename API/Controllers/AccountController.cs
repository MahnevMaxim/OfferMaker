using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Shared;
using Microsoft.EntityFrameworkCore;
using API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNet.Identity;

namespace API.Controllers
{
    public class AccountController : Controller
    {
        private readonly APIContext _context;

        public AccountController(APIContext context)
        {
            _context = context;
        }

        [HttpPost("/token", Name = nameof(AccountGetToken))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        async public Task<ActionResult> AccountGetToken(string username, string password)
        {
            var identity = await GetIdentity(username, password);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var res = await _context.Users.ToListAsync();
            User user = res.FirstOrDefault(x => x.Email == username && x.Pwd == password);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name,
                user = user
            };

            return Ok(response);
        }

        async private Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            var res = await _context.Users.Include(u=>u.Position).ToListAsync();
            User user = res.FirstOrDefault(x => x.Email == username);
            if (user != null)
            {
                var ph = new PasswordHasher();
                var isCurrentHashValid = ph.VerifyHashedPassword(user.Pwd, password);
                if (isCurrentHashValid == PasswordVerificationResult.Success)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                    };
                    foreach(var p in user.Position.Permissions)
                    {
                        claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, p.ToString()));
                    }
                    ClaimsIdentity claimsIdentity =
                    new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                        ClaimsIdentity.DefaultRoleClaimType);
                    return claimsIdentity;
                }
                return null;
            }

            // если пользователя не найдено
            return null;
        }
    }
}
