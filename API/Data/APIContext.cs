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
        public DbSet<Category> Categories { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<OfferHistory> OffersHistory { get; set; }
        public DbSet<OfferTemplate> OfferTemplates { get; set; }
        public DbSet<NomenclatureGroup> NomenclatureGroups { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Hint> Hints { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<Advertising> Advertisings { get; set; }
        public DbSet<ImageGuid> ImageGuids { get; set; }

        public APIContext(DbContextOptions<APIContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(message => Log.EfWrite(message));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            #region Etc

            builder.Entity<Currency>().Property(p => p.Rate).HasPrecision(18, 6);

            builder.Entity<NomenclatureGroup>().Property(p => p.Nomenclatures).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<Nomenclature>>(v));

            builder.Entity<Position>(entity => {
                entity.HasIndex(e => e.PositionName).IsUnique();
            });

            builder.Entity<Position>().Property(p => p.Permissions).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<Permissions>>(v));

            #endregion Etc

            #region User

            builder.Entity<User>().Property(p => p.Image).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Image>(v));

            #endregion

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

            builder.Entity<Offer>().HasAlternateKey(o => o.Guid);

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

            builder.Entity<Offer>().Property(p => p.AdvertisingsUp_).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<Advertising>>(v))
                .HasDefaultValue(new ObservableCollection<Advertising>());

            builder.Entity<Offer>().Property(p => p.AdvertisingsDown_).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<Advertising>>(v))
                .HasDefaultValue(new ObservableCollection<Advertising>());

            builder.Entity<Offer>().Property(p => p.Discount).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Discount>(v));

            builder.Entity<Offer>().Property(p => p.Currency).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Currency>(v));

            builder.Entity<Offer>().Property(n => n.CreateDate).HasDefaultValue(DateTime.UtcNow);

            builder.Entity<Offer>().Property(p => p.Currencies).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<Currency>>(v));

            #endregion Offer

            #region OfferChilds

            builder.Entity<OfferHistory>().HasAlternateKey(o => o.Guid);

            builder.Entity<OfferHistory>().Property(p => p.Customer).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Customer>(v));

            builder.Entity<OfferHistory>().Property(p => p.OfferGroups).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<OfferGroup>>(v));

            builder.Entity<OfferHistory>().Property(p => p.OfferInfoBlocks).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<OfferInfoBlock>>(v));

            builder.Entity<OfferHistory>().Property(p => p.AdvertisingsUp).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<string>>(v));

            builder.Entity<OfferHistory>().Property(p => p.AdvertisingsDown).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<string>>(v));

            builder.Entity<OfferHistory>().Property(p => p.AdvertisingsUp_).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<Advertising>>(v))
                .HasDefaultValue(new ObservableCollection<Advertising>());

            builder.Entity<OfferHistory>().Property(p => p.AdvertisingsDown_).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<Advertising>>(v))
                .HasDefaultValue(new ObservableCollection<Advertising>());

            builder.Entity<OfferHistory>().Property(p => p.Discount).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Discount>(v));

            builder.Entity<OfferHistory>().Property(p => p.Currency).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Currency>(v));

            builder.Entity<OfferHistory>().Property(n => n.CreateDate).HasDefaultValue(DateTime.UtcNow);

            builder.Entity<OfferHistory>().Property(p => p.Currencies).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<Currency>>(v));

            #endregion OfferChilds

            #region OfferTemplate 

            builder.Entity<OfferTemplate>().HasAlternateKey(o => o.Guid);

            builder.Entity<OfferTemplate>().Property(p => p.Customer).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Customer>(v));

            builder.Entity<OfferTemplate>().Property(p => p.OfferGroups).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<OfferGroup>>(v));

            builder.Entity<OfferTemplate>().Property(p => p.OfferInfoBlocks).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<OfferInfoBlock>>(v));

            builder.Entity<OfferTemplate>().Property(p => p.AdvertisingsUp).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<string>>(v));

            builder.Entity<OfferTemplate>().Property(p => p.AdvertisingsDown).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<string>>(v));

            builder.Entity<OfferTemplate>().Property(p => p.AdvertisingsUp_).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<Advertising>>(v))
                .HasDefaultValue(new ObservableCollection<Advertising>());

            builder.Entity<OfferTemplate>().Property(p => p.AdvertisingsDown_).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ObservableCollection<Advertising>>(v))
                .HasDefaultValue(new ObservableCollection<Advertising>());

            builder.Entity<OfferTemplate>().Property(p => p.Discount).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Discount>(v));

            builder.Entity<OfferTemplate>().Property(p => p.Currency).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Currency>(v));

            builder.Entity<OfferTemplate>().Property(n => n.CreateDate).HasDefaultValue(DateTime.UtcNow);

            #endregion OfferTemplate 
        }
    }
}
