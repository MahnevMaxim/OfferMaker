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
    public class CategoriesControllerTest
    {
        public static DbContextOptions<APIContext> dbContextOptions { get; }
        public static string connectionString = @"Server=(localdb)\mssqllocaldb;Database=CategoriesControllerTest;ConnectRetryCount=0";
        ILogger<CategoriesController> logger;
        static TestDataInitializer db;

        static CategoriesControllerTest()
        {
            dbContextOptions = new DbContextOptionsBuilder<APIContext>()
                .UseSqlServer(connectionString)
                .Options;
            APIContext context = new APIContext(dbContextOptions);
            db = new TestDataInitializer();
            db.SeedCategories(context);
        }

        [Fact]
        async public void CategoriesGetTest()
        {
            using (var context = new APIContext(dbContextOptions))
            {
                //Arrange  
                var controller = new CategoriesController(context);

                //Act  
                var response = await controller.CategoriesGet();
                var res = response.Value;

                //Assert 
                Assert.True(res.Count() == context.Categories.Count());
            }
        }

        [Fact]
        async public void CategoriesSaveTest()
        {
            using (var context = new APIContext(dbContextOptions))
            {
                //Arrange  
                var controller = new CategoriesController(context);
                int beginCount = context.Categories.Count();
                var categories = (await controller.CategoriesGet()).Value;

                //Act add
                Category cat = new Category() { Guid = "new_cat_guid", Title = "new_cat_title" };
                var cats = categories.ToList();
                cats.Add(cat);
                var response = await controller.CategoriesSave(cats);

                //Assert 
                Assert.True(context.Categories.Count() == beginCount + 1);

                //Act delete
                int beginCount2 = context.Categories.Count();
                cats.RemoveAt(1);
                var response2 = await controller.CategoriesSave(cats);

                //Assert 
                Assert.True(context.Categories.Count() == beginCount2 - 1);

                //Act edit
                cats[0].Title = "cat0title";
                var response3 = await controller.CategoriesSave(cats);

                //Assert 
                Assert.True(context.Categories.First().Title == "cat0title");
            }
        }
    }
}
