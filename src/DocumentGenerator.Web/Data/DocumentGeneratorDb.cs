using Microsoft.EntityFrameworkCore;
using DocumentGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentGenerator.Web.Data;
using DocumentFormat.OpenXml.Vml;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DocumentGenerator.Web.Helpers;
using System.Reflection.Emit;
using Newtonsoft.Json;

namespace DocumentGenerator.Web.Data
{
   
    public class DocumentGeneratorDb : DbContext
    {

        public string DbPath { get; }
        public DocumentGeneratorDb()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "docdb.db");
        }

        public DocumentGeneratorDb(DbContextOptions<DocumentGeneratorDb> options)
            : base(options)
        {
        }
        public DbSet<DocFunction> DocFunctions { get; set; }
        public DbSet<DocTemplate> DocTemplates { get; set; }
        public DbSet<KeyValueData> KeyValueDatas { get; set; }
       
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<DocTemplate>()
             .Property(e => e.DocFunctions)
             .HasConversion(
                 v => JsonConvert.SerializeObject(v),
                 v => JsonConvert.DeserializeObject<List<long>>(v));
            
            builder.Entity<DocFunction>()
             .Property(e => e.Parameters)
             .HasConversion(
                 v => JsonConvert.SerializeObject(v),
                 v => JsonConvert.DeserializeObject<List<string>>(v));

           
            base.OnModelCreating(builder);
        }

        public override int SaveChanges()
        {
            
            return base.SaveChanges();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
           => options.UseSqlite($"Data Source={DbPath}");
        

    }
}
