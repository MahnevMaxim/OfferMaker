using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using Microsoft.EntityFrameworkCore;
using API.Controllers;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Shared;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace ApiTests
{
    public class ImagesControllerTests
    {
        public static DbContextOptions<APIContext> dbContextOptions { get; }
        public static string connectionString = @"Server=(localdb)\mssqllocaldb;Database=ImagesControllerTests;ConnectRetryCount=0";
        static TestDataInitializer db;

        static ImagesControllerTests()
        {
            dbContextOptions = new DbContextOptionsBuilder<APIContext>()
                .UseSqlServer(connectionString)
                .Options;
            APIContext context = new APIContext(dbContextOptions);
            db = new TestDataInitializer(context);
            db.SeedCategories();
        }

        //[Fact]
        //async public void ImagePostTest()
        //{
        //    using (var context = new APIContext(dbContextOptions))
        //    {
        //        //Arrange  
        //        var controller = new ImagesController(context);

        //        //Act  
        //        var response = await controller.ImagePost();
        //        var res = response.Value;

        //        //Assert 
        //        Assert.True(res.Count() == context.Hints.Count());
        //    }
        //}

        //[Fact]
        //async public void ImageGetTest()
        //{
        //    using (var context = new APIContext(dbContextOptions))
        //    {
        //        //Arrange  
        //        var controller = new ImagesController(context);

        //        //Act  
        //        var response = await controller.HintsGet();
        //        var res = response.Value;

        //        //Assert 
        //        Assert.True(res.Count() == context.Hints.Count());
        //    }
        //}
    }
}
