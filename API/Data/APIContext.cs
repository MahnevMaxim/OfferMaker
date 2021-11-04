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
        public APIContext(DbContextOptions<APIContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Category { get; set; }

        public DbSet<Currency> Currency { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Nomenclature>().Property(p => p.Description).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<string>>(v));

            builder.Entity<User>().Property(p => p.Permissions).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<Permissions>>(v));
        }

        public DbSet<Nomenclature> Nomenclature { get; set; }

        public DbSet<Shared.User> User { get; set; }

        public DbSet<Shared.Customer> Customer { get; set; }
    }
}
