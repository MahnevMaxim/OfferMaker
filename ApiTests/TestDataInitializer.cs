using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using API.Controllers;
using Shared;

namespace ApiTests
{
    public class TestDataInitializer
    {
        public TestDataInitializer()
        {
        }

        public void SeedCurrency(APIContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var one = new Currency() { IsoCode = 100, CharCode = "one", Name = "ssssssss", Rate = 10, Symbol = "563653", RateDatetime = DateTime.Now };
            var two = new Currency() { IsoCode = 101, CharCode = "two", Name = "nnnnnnnnnn", Rate = 20, Symbol = "567457", RateDatetime = DateTime.Now };
            var three = new Currency() { IsoCode = 102, CharCode = "three", Name = "vvvvvvvvv", Rate = 30, Symbol = "58767857", RateDatetime = DateTime.Now };
            var four = new Currency() { IsoCode = 103, CharCode = "four", Name = "ssssssss", Rate = 10, Symbol = "89689689", RateDatetime = DateTime.Now };
            var five = new Currency() { IsoCode = 104, CharCode = "five", Name = "nnnnnnnnnn", Rate = 20, Symbol = "674747", RateDatetime = DateTime.Now };

            context.Currencies.AddRange(one, two, three, four, five);
            context.SaveChanges();
        }

        internal void SeedPositions(APIContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            Position p1 = new Position() { PositionName = "11111"};
            Position p2 = new Position() { PositionName = "22222" };
            Position p3 = new Position() { PositionName = "3333333" };

            context.Positions.AddRange(p1, p2, p3);
            context.SaveChanges();
        }

        internal void SeedAccounts(APIContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            User u1 = new User() { Email = "11111", Pwd = "rrrrr", Role = "role" };
            User u2 = new User() { Email = "2222", Pwd = "tttttt", Role = "role" };
            User u3 = new User() { Email = "3333", Pwd = "yyyyyy", Role = "role" };
            User u4 = new User() { Email = "4444", Pwd = "uu", Role = "role" };
            User u5 = new User() { Email = "555555", Pwd = "jjjjjjjjj", Role = "role" };

            context.Users.AddRange(u1, u2, u3, u4, u5);
            context.SaveChanges();
        }
    }
}
