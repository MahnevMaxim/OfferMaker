using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace API.Data
{
    public class APIContext : DbContext
    {
        public DbSet<Nomenclature> Nomenclature { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Currency> Currency { get; set; }
        public DbSet<Offer> Offer { get; set; }

        public APIContext(DbContextOptions<APIContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Currency>().Property(p => p.Rate).HasPrecision(18, 6);

            builder.Entity<Nomenclature>().Property(p => p.Description).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<string>>(v));

            builder.Entity<Nomenclature>().Property(p => p.Photos).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<string>>(v));

            builder.Entity<User>().Property(p => p.Permissions).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<Permissions>>(v));

            builder.Entity<Offer>().Property(p => p.Images).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<string>>(v));
        }
    }
}
