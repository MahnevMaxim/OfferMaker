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
using Newtonsoft.Json.Linq;

namespace ApiTests
{
    public class AccountControllerTests
    {
        public static DbContextOptions<APIContext> dbContextOptions { get; }
        public static string connectionString = @"Server=(localdb)\mssqllocaldb;Database=AccountControllerTests;ConnectRetryCount=0";
        ILogger<CurrenciesController> logger;
        static TestDataInitializer db;

        static AccountControllerTests()
        {
            dbContextOptions = new DbContextOptionsBuilder<APIContext>()
                .UseSqlServer(connectionString)
                .Options;
            APIContext context = new APIContext(dbContextOptions);
            db = new TestDataInitializer(context);
            db.SeedAccountsAndPositions();
        }

        [Fact]
        async public void GetTokenOkResult()
        {
            using (var context = new APIContext(dbContextOptions))
            {
                //Arrange  
                var controller = new AccountController(context);

                //Act  
                var response = await controller.AccountGetToken(db.testUserLogin, db.testUserPwd);
                string res = ((OkObjectResult)response).Value.ToString();
                JObject jo = JObject.Parse(res);
                string access_token = jo["access_token"].ToString();
                string username = jo["user"]["Email"].ToString();

                //Assert 
                Assert.True(access_token != null);
                Assert.True(username == context.Users.First(u => u.Email == username).Email);
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

        /// <summary>
        /// Проверяем, чтобы токен для экспорта был недоступен, иначе пизда.
        /// </summary>
        [Fact]
        async public void GetTokenForExportNonAccesibleTest()
        {
            using (var context = new APIContext(dbContextOptions))
            {
                //Arrange  
                var controller = new AccountController(context);

                //Act  
                var response = await controller.GetToken();

                //Assert 
                Assert.IsType<BadRequestResult>(response);
            }
        }

        /// <summary>
        /// Чекаем обновление токена и его работоспособность
        /// </summary>
        [Fact]
        async public void AccountUpdateTokenOkResultTest()
        {
            using (var context = new APIContext(dbContextOptions))
            {
                //Arrange  
                var controller = new AccountController(context);
                var controllerPos = new PositionsController(context);
                var responseToken = await controller.AccountGetToken(db.testUserLogin, db.testUserPwd);
                string accessToken = GetTokenFromResponse(responseToken);
                
                //Act  
                var responseUpdateToken = await controller.AccountUpdateToken(accessToken);
                
                //Assert 
                Assert.IsType<OkObjectResult>(responseUpdateToken);
            }
        }

        [Fact]
        async public void AccountUpdateTokenBadResultTest()
        {
            using (var context = new APIContext(dbContextOptions))
            {
                //Arrange  
                var controller = new AccountController(context);

                //Act  
                var response = await controller.GetToken();

                //Assert 
                Assert.IsType<BadRequestResult>(response);
            }
        }

        private string GetTokenFromResponse(ActionResult responseToken)
        {
            string res = ((OkObjectResult)responseToken).Value.ToString();
            JObject jo = JObject.Parse(res);
            return jo["access_token"].ToString();
        }
    }
}
