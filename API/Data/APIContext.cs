using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace API.Data
{
    public class APIContext : DbContext
    {
        public DbSet<INomenclature> Nomenclature { get; set; }
        public DbSet<IUser> User { get; set; }
        public DbSet<ICustomer> Customer { get; set; }
        public DbSet<ICategory> Category { get; set; }
        public DbSet<ICurrency> Currency { get; set; }

        public APIContext(DbContextOptions<APIContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<INomenclature>().Property(p => p.Description).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<string>>(v));

            builder.Entity<INomenclature>().Property(p => p.Photos).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<string>>(v));

            builder.Entity<IUser>().Property(p => p.Permissions).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<Permissions>>(v));
        }
    }
}
