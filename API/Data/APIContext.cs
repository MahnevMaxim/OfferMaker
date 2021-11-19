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
        public DbSet<Nomenclature> Nomenclatures { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<NomenclatureGroup> NomenclatureGroups { get; set; }

        public APIContext(DbContextOptions<APIContext> options)
            : base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(message => L.LW(message));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Currency>().Property(p => p.Rate).HasPrecision(18, 6);

            builder.Entity<Nomenclature>().Property(p => p.Descriptions).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<Description>>(v));

            builder.Entity<Nomenclature>().Property(p => p.Photos).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<string>>(v));

            builder.Entity<User>().Property(p => p.Permissions).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<Permissions>>(v));

            builder.Entity<Offer>().Property(p => p.Images).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<string>>(v));

            builder.Entity<NomenclatureGroup>().Property(p => p.Nomenclatures).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<Nomenclature>>(v));
        }
    }
}
