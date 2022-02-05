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
    public class NomenclatureGroupsControllerTests
    {
        public static DbContextOptions<APIContext> dbContextOptions { get; }
        public static string connectionString = @"Server=(localdb)\mssqllocaldb;Database=NomenclatureGroupsControllerTests;ConnectRetryCount=0";
        static TestDataInitializer db;

        static NomenclatureGroupsControllerTests()
        {
            dbContextOptions = new DbContextOptionsBuilder<APIContext>()
                .UseSqlServer(connectionString)
                .Options;
            APIContext context = new APIContext(dbContextOptions);
            db = new TestDataInitializer(context);
            db.SeedNomenclatureGroups();
        }

        [Fact]
        async public void NomenclatureGroupsGetTest()
        {
            using (var context = new APIContext(dbContextOptions))
            {
                //Arrange  
                var controller = new NomenclatureGroupsController(context);

                //Act  
                var response = await controller.NomenclatureGroupsGet();
                var res = response.Value;

                //Assert 
                Assert.True(res.Count() == context.NomenclatureGroups.Count());
            }
        }

        //[Fact]
        //async public void NomenclatureGroupsSaveTest()
        //{
        //    using (var context = new APIContext(dbContextOptions))
        //    {
        //        //Arrange  
        //        var controller = new HintsController(context);

        //        //Act  
        //        var response = await controller.HintsGet();
        //        var res = response.Value;

        //        //Assert 
        //        Assert.True(res.Count() == context.Hints.Count());
        //    }
        //}


    }
}
