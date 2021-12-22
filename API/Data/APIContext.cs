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
            optionsBuilder.LogTo(message => Log.Write(message));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            #region Etc

            builder.Entity<Currency>().Property(p => p.Rate).HasPrecision(18, 6);

            builder.Entity<User>().Property(p => p.Permissions).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<Permissions>>(v));

            builder.Entity<NomenclatureGroup>().Property(p => p.Nomenclatures).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<Nomenclature>>(v));

            #endregion Etc

            #region Nomenclature

            builder.Entity<Nomenclature>().Property(p => p.Image).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Image>(v));

            builder.Entity<Nomenclature>().Property(p => p.Descriptions).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<Description>>(v));

            builder.Entity<Nomenclature>().Property(p => p.Images).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<Image>>(v));

            builder.Entity<Nomenclature>().Property(n => n.ActualPricePeriod).HasDefaultValue(30);

            builder.Entity<Nomenclature>().Property(n => n.LastChangePriceDate).HasDefaultValue(DateTime.UtcNow);

            #endregion Nomenclature

            #region Offer

            builder.Entity<Offer>().Property(p => p.Customer).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Customer>(v));

            builder.Entity<Offer>().Property(p => p.OfferGroups).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<OfferGroup>>(v));

            builder.Entity<Offer>().Property(p => p.OfferInfoBlocks).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<OfferInfoBlock>>(v));

            builder.Entity<Offer>().Property(p => p.AdvertisingsUp).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<string>>(v));

            builder.Entity<Offer>().Property(p => p.AdvertisingsDown).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<string>>(v));

            builder.Entity<Offer>().Property(p => p.Discount).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Discount>(v));

            builder.Entity<Offer>().Property(p => p.Currency).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Currency>(v));

            builder.Entity<Offer>().Property(n => n.CreateDate).HasDefaultValue(DateTime.UtcNow);

            #endregion Offer
        }
    }
}
