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
    public class OfferTemplatesControllerTests
    {
        public static DbContextOptions<APIContext> dbContextOptions { get; }
        public static string connectionString = @"Server=(localdb)\mssqllocaldb;Database=OfferTemplatesControllerTests;ConnectRetryCount=0";
        static TestDataInitializer db;

        static OfferTemplatesControllerTests()
        {
            dbContextOptions = new DbContextOptionsBuilder<APIContext>()
                .UseSqlServer(connectionString)
                .Options;
            APIContext context = new APIContext(dbContextOptions);
            db = new TestDataInitializer(context);
            db.SeedCurrency();
            db.SeedOfferTemplates();
        }

        [Fact]
        async public void OfferTemplatesGetTest()
        {
            using (var context = new APIContext(dbContextOptions))
            {
                //Arrange  
                var controller = new OfferTemplatesController(context);

                //Act  
                var response = await controller.OfferTemplatesGet();
                var res = response.Value;

                //Assert 
                Assert.True(res.Count() == context.OfferTemplates.Count());
            }
        }

        [Fact]
        async public void OfferTemplatePostTest()
        {
            using (var context = new APIContext(dbContextOptions))
            {
                //Arrange  
                var controller = new OfferTemplatesController(context);
                int beginCount = context.OfferTemplates.Count();
                var offerTemplate = new OfferTemplate()
                {
                    CreateDate = DateTime.Now,
                    Currency = context.Currencies.First(),
                    Customer = new Customer(),
                    Guid = Guid.NewGuid().ToString(),
                    OfferName = "3333"
                };

                //Act  
                var response = await controller.OfferTemplatePost(offerTemplate);
                var res = response.Value;

                //Assert 
                Assert.True(context.OfferTemplates.Count() == beginCount + 1);
                Assert.True(((CreatedAtActionResult)response.Result).StatusCode == 201);
            }
        }

        [Fact]
        async public void OfferTemplatesDeleteTest()
        {
            using (var context = new APIContext(dbContextOptions))
            {
                //Arrange  
                var controller = new OfferTemplatesController(context);
                int beginCount = context.OfferTemplates.Count();

                //Act  
                var response = await controller.OfferTemplateDelete(1);

                //Assert 
                Assert.True(context.OfferTemplates.Count() == beginCount - 1);
            }
        }

        [Fact]
        async public void OfferTemplatesEditTest()
        {
            using (var context = new APIContext(dbContextOptions))
            {
                //Arrange  
                var controller = new OfferTemplatesController(context);
                var editTemplate = context.OfferTemplates.AsNoTracking().First();
                var newName = Guid.NewGuid().ToString();
                OfferTemplate ot = new OfferTemplate()
                {
                    Id = editTemplate.Id,
                    CreateDate = DateTime.Now,
                    Currency = context.Currencies.First(),
                    Customer = new Customer(),
                    Guid = Guid.NewGuid().ToString(),
                    OfferName = newName
                };

                //Act  
                var response = await controller.OfferTemplateEdit(editTemplate.Id, ot);

                //Assert 
                Assert.True(context.OfferTemplates.First().OfferName == newName);
            }
        }
    }
}
