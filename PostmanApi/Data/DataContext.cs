using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PostmanApi.Interfaces;
using PostmanApi.Models;

namespace PostmanApi.Data
{
    public class DataContext : DbContext, IUnitOfWork
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            //Database.SetCommandTimeout(300);
            //ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<Contact> Contacts { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.EnableSensitiveDataLogging();
        //    optionsBuilder.EnableDetailedErrors();
        //    optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

        //    //optionsBuilder.LogTo(Console.WriteLine);
        //    //optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
        //    //optionsBuilder
        //    //    .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Name });
        //    optionsBuilder
        //        .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Name, DbLoggerCategory.Database.Name, DbLoggerCategory.Database.Command.Name }, LogLevel.Information);
        //    optionsBuilder.LogTo(message => Debug.WriteLine(message));
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ContactConfiguration());
        }
    }
}
