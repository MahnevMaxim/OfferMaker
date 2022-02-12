using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace DatabaseExporter.Nomenclatures
{
    public partial class CommercialdbContext : DbContext
    {
        public CommercialdbContext()
        {
        }

        public CommercialdbContext(DbContextOptions<CommercialdbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Description> Descriptions { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySQL("Server=localhost;Database=commercialdb;Uid=root;Pwd=7yfcntyf7;persist security info=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Description>(entity =>
            {
                entity.HasKey(e => new { e.ItemId, e.Number })
                    .HasName("PRIMARY");

                entity.ToTable("description");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.Descriptions)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_description_to_item");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.ToTable("item");

                entity.Property(e => e.MarkUp).HasDefaultValueSql("'1'");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(210);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(45);

                entity.Property(e => e.Foto).HasColumnType("mediumblob");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(45);

                entity.Property(e => e.Tel1)
                    .IsRequired()
                    .HasMaxLength(45);

                entity.Property(e => e.Tel2)
                    .IsRequired()
                    .HasMaxLength(45);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
