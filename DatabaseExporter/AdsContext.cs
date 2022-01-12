using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DatabaseExporter
{
    /// <summary>
    /// Взаимодействие с базой данных в Entity Framework Core происходит посредством специального класса - контекста данных
    /// </summary>
    /// DbContext: определяет контекст данных, используемый для взаимодействия с базой данных
    public class AdsContext : DbContext
    {
        //DbSet/DbSet<TEntity>: представляет набор объектов, которые хранятся в базе данных
       
        public DbSet<Ad> Ads { get; set; }

        public AdsContext(DbContextOptions<AdsContext> options)
            : base(options)
        {
            // по умолчанию у нас нет базы данных.
            // Поэтому в конструкторе класса контекста определен вызов метода Database.EnsureCreated(),
            // который при создании контекста автоматически проверит наличие базы данных и, если она отсуствует, создаст ее.
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
        }
    }

    public enum Store
    {
        Ali,
        Citilink,
        Avito,
        Au,
        Youla
    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.4.6.0 (Newtonsoft.Json v12.0.0.0)")]
    public class Ad
    {
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public int Id { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("externalId")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = false)]
        public string ExternalId { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("title")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = false)]
        public string Title { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("description")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = false)]
        public string Description { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("price")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = false)]
        public double Price { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("imgLink")]
        public string ImgLink { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("url")]
        public string Url { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("imgPath")]
        public string ImgPath { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("uploadImageStatus")]
        public bool UploadImageStatus { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("time")]
        public DateTime Time { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("catId")]
        public int? CatId { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("store")]
        public Store Store { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("location")]
        public string Location { get; set; }

        public override string ToString() => Title;
    }
}
