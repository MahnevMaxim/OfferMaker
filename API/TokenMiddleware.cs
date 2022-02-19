using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using API.Data;
using Microsoft.EntityFrameworkCore;
using Shared;
using Microsoft.AspNet.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, APIContext apiContext)
        {
            string destination = context.Request.Path;

            //если обновление токена или авторизация, то пропускаем запрос
            if(destination== "/updatetoken" || destination== "/token" || destination == "/api/Download")
            {
                await _next.Invoke(context);
            }
            else
            {
                //если экспорт, то пропускаем
                var identity = context.User.Identity as ClaimsIdentity;
                if (identity.Name== "Export")
                {
                    await _next.Invoke(context);
                }
                else
                {
                    //пропускаем запрос только если есть такой активный токен
                    //вся эта хуйня, только для того, чтобы сразу отваливались права, когда их кто-нибудь изменил
                    var token = context.Request.Headers["Authorization"].ToString().Split(' ')[1];
                    var res = apiContext.Users.Include(u => u.Account).ToList().FirstOrDefault(u => u.Account?.Token == token && u.Account.IsTokenActive);
                    if (res == null)
                    {
                        context.Response.StatusCode = 403;
                        Log.Write("TokenMiddleware error");
                        await context.Response.WriteAsync("Token is invalid");
                    }
                    else
                    {
                        Log.Write("TokenMiddleware good");
                        await _next.Invoke(context);
                    }
                }
            }
        }
    }
}
