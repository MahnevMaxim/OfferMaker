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
    public class BannersControllerTests
    {
        public static DbContextOptions<APIContext> dbContextOptions { get; }
        public static string connectionString = @"Server=(localdb)\mssqllocaldb;Database=BannersControllerTests;ConnectRetryCount=0";
        ILogger<BannersController> logger;
        static TestDataInitializer db;

        static BannersControllerTests()
        {
            dbContextOptions = new DbContextOptionsBuilder<APIContext>()
                .UseSqlServer(connectionString)
                .Options;
            APIContext context = new APIContext(dbContextOptions);
            db = new TestDataInitializer(context);
            db.SeedBanners();
        }

        [Fact]
        async public void BannersGetTest()
        {
            using (var context = new APIContext(dbContextOptions))
            {
                //Arrange  
                var controller = new BannersController(context);

                //Act  
                var response = await controller.BannersGet();
                var res = response.Value;

                //Assert 
                Assert.True(res.Count() == context.Banners.Where(b=>!b.IsDeleted).Count());
            }
        }

        [Fact]
        async public void BannersPostTest()
        {
            using (var context = new APIContext(dbContextOptions))
            {
                //Arrange  
                var controller = new BannersController(context);
                int beginCount = context.Banners.Count();

                //Act  
                var response = await controller.BannerPost(new Banner() { Guid = "new_banner_guid" });

                //Assert 
                Assert.True(context.Banners.Count() == beginCount + 1);
                Assert.True(((CreatedAtActionResult)response.Result).StatusCode == 201);
            }
        }

        [Fact]
        async public void BannersDeleteTest()
        {
            using (var context = new APIContext(dbContextOptions))
            {
                //Arrange  
                var controller = new BannersController(context);
                int beginCount = context.Banners.Count();

                //Act  
                var response = await controller.BannerDelete(1);

                //Assert 
                Assert.True(context.Banners.First(a => a.Id == 1).IsDeleted);
                Assert.True(context.Banners.Count() == beginCount);
            }
        }

    }
}
