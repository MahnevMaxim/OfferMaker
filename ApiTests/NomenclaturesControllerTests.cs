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
    public class NomenclaturesControllerTests
    {
        public static DbContextOptions<APIContext> dbContextOptions { get; }
        public static string connectionString = @"Server=(localdb)\mssqllocaldb;Database=NomenclaturesControllerTests;ConnectRetryCount=0";
        static TestDataInitializer db;

        static NomenclaturesControllerTests()
        {
            dbContextOptions = new DbContextOptionsBuilder<APIContext>()
                .UseSqlServer(connectionString)
                .Options;
            APIContext context = new APIContext(dbContextOptions);
            db = new TestDataInitializer(context);
            db.SeedCategories();
        }
    }
}
