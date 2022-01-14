using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly APIContext _apiContext;

        public TokenMiddleware(RequestDelegate next)
        {
            _next = next;
            //_apiContext = apiContext;
        }

        public async Task InvokeAsync(HttpContext context, APIContext apiContext)
        {
            var token = context.Request.Query["token"];
            var res2 = apiContext.Users.Include(u => u.Account).ToList();
            //var res = apiContext.Users.Include(u => u.Account).Where(u => u.Account?.Token == token).FirstOrDefault();
            if (token != "12345678")
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Token is invalid");
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
}
