using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreBackend.Api.Configuration;

namespace CoreBackend.Api.Entities
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options):base(options)
        {
            //Database.EnsureCreated();
            Database.Migrate();
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Material> Materials { get; set; }

        ///// <summary>
        ///// 数据库连接
        ///// </summary>
        ///// <param name="optionsBuilder"></param>
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("xxx connection string");
        //    base.OnConfiguring(optionsBuilder);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasKey(x => x.Id);
            modelBuilder.Entity<Product>().Property(x => x.Name).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Product>().Property(x => x.Price).HasColumnType("decimal(8,2)");
            //modelBuilder.ApplyConfiguration(new  ());
            //modelBuilder.ApplyConfiguration(new MaterialConfiguration());
        }
    }
}
