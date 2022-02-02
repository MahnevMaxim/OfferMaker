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

namespace ApiTests
{
    public class PositionsControllerTests
    {
        public static DbContextOptions<APIContext> dbContextOptions { get; }
        public static string connectionString = @"Server=(localdb)\mssqllocaldb;Database=PositionsControllerTests;ConnectRetryCount=0";

        static PositionsControllerTests()
        {
            dbContextOptions = new DbContextOptionsBuilder<APIContext>()
                .UseSqlServer(connectionString)
                .Options;
            APIContext context = new APIContext(dbContextOptions);
            TestDataInitializer db = new TestDataInitializer();
            db.SeedAccountsAndPositions(context);
        }

        [Fact]
        async public void GetPositions()
        {
            using (var context = new APIContext(dbContextOptions))
            {
                //Arrange  
                var controller = new PositionsController(context);

                //Act  
                var response = await controller.PositionsGet();
                List<Position> res = new List<Position>(((ActionResult<IEnumerable<Position>>)response).Value);
                int count = res.Count;

                //Assert 
                Assert.True(count==3);
            }
        }
    }
}
