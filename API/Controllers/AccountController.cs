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
using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Controllers
{
    public class AccountController : Controller
    {
        private readonly APIContext _context;
        private readonly string invalidUsername = "Invalid username or password.";

        public AccountController(APIContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получение токена по логину и паролю.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost("/token", Name = nameof(AccountGetToken))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        async public Task<ActionResult> AccountGetToken(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return BadRequest(new { errorText = invalidUsername });
            }

            ClaimsIdentity identity = await GetIdentity(username, password);
            if (identity == null)
            {
                return BadRequest(new { errorText = invalidUsername });
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

            var res = await _context.Users.Include(u => u.Account).ToListAsync();
            User user = res.FirstOrDefault(x => x.Email == identity.Name);
            if (user.Position.Id == 0)
                user.Position = null;
            if (user.Account == null)
            {
                user.Account = new Account() { Token = encodedJwt, IsTokenActive = true };
            }
            else
            {
                user.Account.Token = encodedJwt;
                user.Account.IsTokenActive = true;
            }

            var ph = new PasswordHasher();
            user.Account.Password = ph.HashPassword(password);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest(new { errorText = ex.StackTrace });
            }

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name,
                user = user
            };

            JsonSerializerOptions options = new() { ReferenceHandler = ReferenceHandler.Preserve };
            string responseJson = JsonSerializer.Serialize(response, options);

            return Ok(responseJson);
        }

        /// <summary>
        /// Токен для экспорта, на рабочем сервере закомментировать или удалить.
        /// </summary>
        /// <returns></returns>
        [HttpGet("/token", Name = nameof(GetToken))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        async public Task<ActionResult> GetToken()
        {
            return BadRequest();

            var claims = new List<Claim> { new Claim(ClaimsIdentity.DefaultNameClaimType, "Export") };
            claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, Permissions.CanAll.ToString()));

            ClaimsIdentity claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: claimsIdentity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = "Export"
            };

            JsonSerializerOptions options = new() { ReferenceHandler = ReferenceHandler.Preserve };
            string responseJson = JsonSerializer.Serialize(response, options);

            return Ok(responseJson);
        }

        /// <summary>
        /// Обновление токена перед запуском приложения.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("/updatetoken", Name = nameof(AccountUpdateToken))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        async public Task<ActionResult> AccountUpdateToken(string token)
        {
            var identity = await GetIdentity(token);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid token." });
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

            var users = await _context.Users.ToListAsync();
            User user = users.FirstOrDefault(x => x.Account?.Token == token);

            if (user.Account == null)
            {
                user.Account = new Account() { Token = encodedJwt, IsTokenActive = true };
            }
            else
            {
                user.Account.Token = encodedJwt;
                user.Account.IsTokenActive = true;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest(new { errorText = ex.StackTrace });
            }

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name,
                user = user
            };

            return Ok(response);
        }

        async private Task<ClaimsIdentity> GetIdentity(string token)
        {
            var users = await _context.Users.Include(u => u.Position).Include(u => u.Account).ToListAsync();
            User user = users.FirstOrDefault(x => x.Account?.Token == token && x.Account.IsTokenActive);

            if (user != null)
            {
                var claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                    };
                foreach (var p in user.Position.Permissions)
                {
                    claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, p.ToString()));
                }
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }

        async private Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            var res = await _context.Users.Include(u => u.Position).Include(u => u.Account).ToListAsync();
            User user = res.FirstOrDefault(x => x.Email == username);

            if (user != null)
            {
                var ph = new PasswordHasher();
                var isCurrentHashValid = ph.VerifyHashedPassword(user.Account.Password, password);
                if (isCurrentHashValid == PasswordVerificationResult.Success)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                    };

                    if (user.Position != null)
                    {
                        foreach (var p in user.Position.Permissions)
                        {
                            claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, p.ToString()));
                        }
                    }
                    else
                    {
                        user.Position = new Position() { Permissions = new System.Collections.ObjectModel.ObservableCollection<Permissions>() };
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
