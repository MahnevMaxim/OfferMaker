using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using API.Controllers;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Shared;
using System.Text.Json;

namespace ApiTests.AccountControllerTests
{
    public class AccountControllerGetTokenTests
    {
        public static DbContextOptions<APIContext> dbContextOptions { get; }
        public static string connectionString = @"Server=(localdb)\mssqllocaldb;Database=AccountControllerTests;ConnectRetryCount=0";
        ILogger<CurrenciesController> logger;

        static AccountControllerGetTokenTests()
        {
            dbContextOptions = new DbContextOptionsBuilder<APIContext>()
                .UseSqlServer(connectionString)
                .Options;
            APIContext context = new APIContext(dbContextOptions);
            TestDataInitializer db = new TestDataInitializer();
            db.SeedAccounts(context);
        }

        [Fact]
        async public void GetTokenOkResult()
        {
            using (var context = new APIContext(dbContextOptions))
            {
                //Arrange  
                var controller = new AccountController(context);

                //Act  
                var response = await controller.AccountGetToken(context.Users.First().Email, context.Users.First().Pwd);
                object res = ((OkObjectResult)response).Value;
                Type type = res.GetType();
                string access_token = (string)type.GetProperty("access_token")?.GetValue(res, null);
                string username = (string)type.GetProperty("username")?.GetValue(res, null);

                //Assert 
                Assert.True(access_token!=null);
                Assert.True(username== context.Users.First().Email);
            }
        }

        [Fact]
        async public void GetTokenBadRequestResult()
        {
            using (var context = new APIContext(dbContextOptions))
            {
                //Arrange  
                var controller = new AccountController(context);

                //Act  
                var response1 = await controller.AccountGetToken("", "");
                var response2 = await controller.AccountGetToken(null, null);
                var response3 = await controller.AccountGetToken("ww", "");
                var response4 = await controller.AccountGetToken("ww", "ee");
                var response5 = await controller.AccountGetToken(context.Users.First().Email, "");
                var response6 = await controller.AccountGetToken(context.Users.First().Email, null);
                var response7 = await controller.AccountGetToken(context.Users.First().Email, "ee");

                //Assert 
                Assert.IsType<BadRequestObjectResult>(response1);
                Assert.IsType<BadRequestObjectResult>(response2);
                Assert.IsType<BadRequestObjectResult>(response3);
                Assert.IsType<BadRequestObjectResult>(response4);
                Assert.IsType<BadRequestObjectResult>(response5);
                Assert.IsType<BadRequestObjectResult>(response6);
                Assert.IsType<BadRequestObjectResult>(response7);
            }
        }
    }
}
