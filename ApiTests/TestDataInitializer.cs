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
using Microsoft.AspNet.Identity;

namespace ApiTests
{
    public class TestDataInitializer
    {
        public string testUserLogin = "TestUser";
        public string testUserPwd = "55555555555555";

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

            Position p1 = new Position() { PositionName = "11111" };
            Position p2 = new Position() { PositionName = "22222" };
            Position p3 = new Position() { PositionName = "3333333" };

            context.Positions.AddRange(p1, p2, p3);
            context.SaveChanges();
        }

        internal void SeedAccounts(APIContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var ph = new PasswordHasher();
            string pwd1 = ph.HashPassword("rrergrrr1");
            string pwd2 = ph.HashPassword("rrwrrr2");
            string pwd3 = ph.HashPassword("rrrsedgrr3");
            string pwd4 = ph.HashPassword("rrsdcrrr4");
            string pwd5 = ph.HashPassword("rrrergrr5");
            string testUserPwdHash = ph.HashPassword(testUserPwd);

            User u1 = new User() { Email = "11111", Account = new Account() { Password = pwd1 } };
            User u2 = new User() { Email = "2222", Account = new Account() { Password = pwd2 } };
            User u3 = new User() { Email = "3333", Account = new Account() { Password = pwd3 } };
            User u4 = new User() { Email = "4444", Account = new Account() { Password = pwd4 } };
            User u5 = new User() { Email = "555555", Account = new Account() { Password = pwd5 } };
            User testUser = new User() { Email = testUserLogin, Account = new Account() { Password = testUserPwdHash } };

            context.Users.AddRange(u1, u2, u3, u4, u5, testUser);
            context.SaveChanges();
        }
    }
}
