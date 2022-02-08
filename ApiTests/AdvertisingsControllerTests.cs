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
    public class AdvertisingsControllerTests
    {
        public static DbContextOptions<APIContext> dbContextOptions { get; }
        public static string connectionString = @"Server=(localdb)\mssqllocaldb;Database=AdvertisingsControllerTests;ConnectRetryCount=0";
        ILogger<AdvertisingsController> logger;
        static TestDataInitializer db;

        static AdvertisingsControllerTests()
        {
            dbContextOptions = new DbContextOptionsBuilder<APIContext>()
                .UseSqlServer(connectionString)
                .Options;
            APIContext context = new APIContext(dbContextOptions);
            db = new TestDataInitializer(context);
            db.SeedAdvertisings();
        }

        [Fact]
        async public void AdvertisingsGetTest()
        {
            using (var context = new APIContext(dbContextOptions))
            {
                //Arrange  
                var controller = new AdvertisingsController(context);

                //Act  
                var response = await controller.AdvertisingsGet();
                var res = response.Value;
                var rr = context.Advertisings.Where(a => !a.IsDeleted).ToList();

                //Assert 
                Assert.True(res.Count() == context.Advertisings.Where(a=>!a.IsDeleted).Count());
            }
        }

        [Fact]
        async public void AdvertisingsPostTest()
        {
            using (var context = new APIContext(dbContextOptions))
            {
                //Arrange  
                var controller = new AdvertisingsController(context);
                int beginCount = context.Advertisings.Count();

                //Act  
                var response = await controller.AdvertisingPost(new Advertising() { Guid = "new_advertising" });

                //Assert 
                Assert.True(context.Advertisings.Count() == beginCount + 1);
                Assert.True(((CreatedAtActionResult)response.Result).StatusCode == 201);
            }
        }

        /// <summary>
        /// Ожидаемый результат - реклама будет помечена на удаление
        /// </summary>
        [Fact]
        async public void AdvertisingsDeleteTest()
        {
            using (var context = new APIContext(dbContextOptions))
            {
                //Arrange  
                var controller = new AdvertisingsController(context);
                int beginCount = context.Advertisings.Count();

                //Act  
                var response = await controller.AdvertisingDelete(1);

                //Assert 
                Assert.True(context.Advertisings.First(a=>a.Id==1).IsDeleted);
                Assert.True(context.Advertisings.Count() == beginCount);
            }
        }
    }
}
