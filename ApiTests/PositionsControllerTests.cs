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
            TestDataInitializer db = new TestDataInitializer(context);
            db.SeedAccountsAndPositions();
        }

        [Fact]
        async public void GetPositionsTest()
        {
            using (var context = new APIContext(dbContextOptions))
            {
                //Arrange  
                var controller = new PositionsController(context);

                //Act  
                var response = await controller.PositionsGet();
                List<Position> res = new List<Position>(((ActionResult<IEnumerable<Position>>)response).Value);

                //Assert 
                Assert.True(res.Count == context.Positions.Count());
            }
        }

        [Fact]
        async public void PositionsEditTest()
        {
            using (var context = new APIContext(dbContextOptions))
            {
                //Arrange  
                var controller = new PositionsController(context);
                string editedName = "editedName";
                var positionsForEdit = new List<Position>()
                {
                    new Position(){ Id=1,PositionName=editedName},
                    new Position(){ Id=2,PositionName=editedName},
                    new Position(){ Id=3,PositionName=editedName}
                };

                //Act  
                var response = await controller.PositionsEdit(positionsForEdit);

                //Assert 
                Assert.True(context.Positions.ToList()[0].PositionName == editedName);
                Assert.True(context.Positions.ToList()[1].PositionName == editedName);
                Assert.True(context.Positions.ToList()[2].PositionName == editedName);
            }
        }

        [Fact]
        async public void PositionsPostTest()
        {
            using (var context = new APIContext(dbContextOptions))
            {
                //Arrange  
                var controller = new PositionsController(context);
                int beginCount = context.Positions.Count();

                //Act  
                var response = await controller.PositionPost(new Position() { PositionName = "newPositionName" });

                //Assert 
                Assert.True(context.Positions.Count() == beginCount + 1);
            }
        }

        [Fact]
        async public void PositionDeleteTest()
        {
            using (var context = new APIContext(dbContextOptions))
            {
                //Arrange  
                var controller = new PositionsController(context);
                int beginCount = context.Positions.Count();

                //Act  
                var response = await controller.PositionDelete(1);

                //Assert 
                Assert.True(context.Positions.Count() == beginCount - 1);
            }
        }

        [Fact]
        async public void PositionPostTest()
        {
            using (var context = new APIContext(dbContextOptions))
            {
                //Arrange  
                var controller = new PositionsController(context);
                int beginCount = context.Positions.Count();
                string newPositionName = "newPositionName2";

                //Act  
                var response = await controller.PositionPost(new Position() { PositionName = newPositionName });

                //Assert 
                Assert.True(context.Positions.Count() == beginCount + 1);
            }
        }
    }
}
