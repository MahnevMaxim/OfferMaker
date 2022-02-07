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
using System.Collections.ObjectModel;

namespace ApiTests
{
    public class TestDataInitializer
    {
        public string testUserLogin = "TestUser";
        public string testUserPwd = "55555555555555";
        APIContext context;

        public TestDataInitializer(APIContext context)
        {
            this.context = context;
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        public void SeedCurrency()
        {
            var one = new Currency() { IsoCode = 100, CharCode = "one", Name = "ssssssss", Rate = 10, Symbol = "563653", RateDatetime = DateTime.Now };
            var two = new Currency() { IsoCode = 101, CharCode = "two", Name = "nnnnnnnnnn", Rate = 20, Symbol = "567457", RateDatetime = DateTime.Now };
            var three = new Currency() { IsoCode = 102, CharCode = "three", Name = "vvvvvvvvv", Rate = 30, Symbol = "58767857", RateDatetime = DateTime.Now };
            var four = new Currency() { IsoCode = 103, CharCode = "four", Name = "ssssssss", Rate = 10, Symbol = "89689689", RateDatetime = DateTime.Now };
            var five = new Currency() { IsoCode = 104, CharCode = "five", Name = "nnnnnnnnnn", Rate = 20, Symbol = "674747", RateDatetime = DateTime.Now };

            context.Currencies.AddRange(one, two, three, four, five);
            context.SaveChanges();
        }

        internal void SeedOfferTemplates()
        {
            var currency = new Currency() { IsoCode = 100, CharCode = "one", Name = "ssssssss", Rate = 10, Symbol = "563653", RateDatetime = DateTime.Now };
            var customer = new Customer();

            var one = new OfferTemplate() { CreateDate = DateTime.Now, Currency = currency, Customer = customer, Guid = "ddd5ddd", OfferName = "3333" };
            var two = new OfferTemplate() { CreateDate = DateTime.Now, Currency = currency, Customer = customer, Guid = "dddd6dd", OfferName = "3333" };
            var three = new OfferTemplate() { CreateDate = DateTime.Now, Currency = currency, Customer = customer, Guid = "dd7dddd", OfferName = "3333" };

            context.OfferTemplates.AddRange(one, two, three);
            context.SaveChanges();
        }

        internal void SeedNomenclatureGroups()
        {
            var oneNom = new Nomenclature() { CostPrice = 10, CurrencyCharCode = "RUB", Markup = 2, Title = "Title1" };
            var twoNom = new Nomenclature() { CostPrice = 20, CurrencyCharCode = "RUB", Markup = 2, Title = "Title2" };
            var threeNom = new Nomenclature() { CostPrice = 30, CurrencyCharCode = "RUB", Markup = 2, Title = "Title3" };
            var fourNom = new Nomenclature() { CostPrice = 40, CurrencyCharCode = "RUB", Markup = 2, Title = "Title4" };
            var fiveNom = new Nomenclature() { CostPrice = 50, CurrencyCharCode = "RUB", Markup = 2, Title = "Title5" };
            var sixNom = new Nomenclature() { CostPrice = 60, CurrencyCharCode = "RUB", Markup = 2, Title = "Title6" };

            context.Nomenclatures.AddRange(oneNom, twoNom, threeNom, fourNom, fiveNom, sixNom);
            context.SaveChanges();

            var one = new NomenclatureGroup() { Name = "name", Nomenclatures = new ObservableCollection<Nomenclature>() { sixNom, twoNom } };
            var two = new NomenclatureGroup() { Name = "name2", Nomenclatures = new ObservableCollection<Nomenclature>() { oneNom, twoNom } };
            var three = new NomenclatureGroup() { Name = "name3", Nomenclatures = new ObservableCollection<Nomenclature>() { threeNom, twoNom } };
            var four = new NomenclatureGroup() { Name = "name4", Nomenclatures = new ObservableCollection<Nomenclature>() { oneNom, fourNom } };
            var five = new NomenclatureGroup() { Name = "name5", Nomenclatures = new ObservableCollection<Nomenclature>() { fiveNom, twoNom } };
            var six = new NomenclatureGroup() { Name = "name6", Nomenclatures = new ObservableCollection<Nomenclature>() { oneNom, sixNom } };

            context.NomenclatureGroups.AddRange(one, two, three, four, five, six);
            context.SaveChanges();
        }

        internal void SeedHints()
        {
            var one = new Hint() { HintString = "fsdwre87h98h8hh98h98989j8ojik0o--ko09uhuyg" };
            var two = new Hint() { HintString = "nhhn87h98h8hh98h98989j8ojik0osdfsfr--ko09uhuyg" };
            var three = new Hint() { HintString = "nhdfng87h98h8hh98h98989j8ojidsfdsfbk0o--ko09uhuyg" };

            context.Hints.AddRange(one, two, three);
            context.SaveChanges();
        }

        internal void SeedCategories()
        {
            var one = new Category() { Guid = "fsdwre87h98h8hh98h98989j8ojik0o--ko09uhuyg", Title = "new_cat_title1" };
            var two = new Category() { Guid = "nhhn87h98h8hh98h98989j8ojik0osdfsfr--ko09uhuyg", Title = "new_cat_title2" };
            var three = new Category() { Guid = "nhdfng87h98h8hh98h98989j8ojidsfdsfbk0o--ko09uhuyg", Title = "new_cat_title3" };

            context.Categories.AddRange(one, two, three);
            context.SaveChanges();
        }

        internal void SeedBanners()
        {
            var one = new Banner() { Guid = "fsdwre87h98h8hh98h98989j8ojik0o--ko09uhuyg" };
            var two = new Banner() { Guid = "nhhn87h98h8hh98h98989j8ojik0osdfsfr--ko09uhuyg" };
            var three = new Banner() { Guid = "nhdfng87h98h8hh98h98989j8ojidsfdsfbk0o--ko09uhuyg" };

            context.Banners.AddRange(one, two, three);
            context.SaveChanges();
        }

        internal void SeedAdvertisings()
        {
            var one = new Advertising() { Guid = "87h98h8hh98h98989j8ojik0o--ko09uhuyg" };
            var two = new Advertising() { Guid = "87h98h8hh98h98989j8ojik0osdfsfr--ko09uhuyg" };
            var three = new Advertising() { Guid = "87h98h8hh98h98989j8ojidsfdsfbk0o--ko09uhuyg" };

            context.Advertisings.AddRange(one, two, three);
            context.SaveChanges();
        }

        internal void SeedAccountsAndPositions()
        {
            Position p1 = new Position() { PositionName = "11111", Permissions = new ObservableCollection<Permissions>() };
            Position p2 = new Position() { PositionName = "22222", Permissions = new ObservableCollection<Permissions>() };
            Position p3 = new Position() { PositionName = "33333", Permissions = new ObservableCollection<Permissions>() };

            context.Positions.AddRange(p1, p2, p3);
            context.SaveChanges();

            var ph = new PasswordHasher();
            string pwd1 = ph.HashPassword("rrergrrr1");
            string pwd2 = ph.HashPassword("rrwrrr2");
            string pwd3 = ph.HashPassword("rrrsedgrr3");
            string pwd4 = ph.HashPassword("rrsdcrrr4");
            string pwd5 = ph.HashPassword("rrrergrr5");
            string testUserPwdHash = ph.HashPassword(testUserPwd);

            User u1 = new User() { Email = "11111", Account = new Account() { Password = pwd1 }, Position = p1 };
            User u2 = new User() { Email = "2222", Account = new Account() { Password = pwd2 }, Position = p1 };
            User u3 = new User() { Email = "3333", Account = new Account() { Password = pwd3 }, Position = p1 };
            User u4 = new User() { Email = "4444", Account = new Account() { Password = pwd4 }, Position = p1 };
            User u5 = new User() { Email = "555555", Account = new Account() { Password = pwd5 }, Position = p1 };

            Position pos = context.Positions.First();
            User testUser = new User() { Email = testUserLogin, Account = new Account() { Password = testUserPwdHash } };
            testUser.Position = pos;

            context.Users.AddRange(u1, u2, u3, u4, u5);
            context.Users.Add(testUser);
            context.SaveChanges();
        }
    }
}
