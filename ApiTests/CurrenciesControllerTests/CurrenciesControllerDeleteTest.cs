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

namespace ApiTests.CurrenciesControllerTests
{
    public class CurrenciesControllerDeleteTest
    {
        public static DbContextOptions<APIContext> dbContextOptions { get; }
        public static string connectionString = @"Server=(localdb)\mssqllocaldb;Database=CurrenciesControllerPutTest;ConnectRetryCount=0";
        ILogger<CurrenciesController> logger;

        static CurrenciesControllerDeleteTest()
        {
            dbContextOptions = new DbContextOptionsBuilder<APIContext>()
                .UseSqlServer(connectionString)
                .Options;
            APIContext context = new APIContext(dbContextOptions);
            TestDataInitializer db = new TestDataInitializer();
            db.SeedCurrency(context);
        }

        [Fact]
        async public void DeleteCurrencyOkResult()
        {
            using (var context = new APIContext(dbContextOptions))
            {
                //Arrange  
                var controller = new CurrenciesController(context);
                var postId = 2;

                //Act  
                var request = await controller.DeleteCurrency(postId);
                var res = await controller.GetCurrency(postId);
                var stCode = ((NotFoundResult)((ActionResult<Currency>)res).Result).StatusCode;

                //Assert 
                Assert.IsType<NoContentResult>(request);
                Assert.True(stCode == 404);
            }
        }

        [Fact]
        async public void DeleteCurrencyNotFound()
        {
            using (var context = new APIContext(dbContextOptions))
            {
                //Arrange  
                var controller = new CurrenciesController(context);

                //Act  
                var deletedCurrencyResult = await controller.DeleteCurrency(666);

                //Assert 
                Assert.IsType<NotFoundResult>(deletedCurrencyResult);
            }
        }
    }
}
