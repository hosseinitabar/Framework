using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Holism.Framework;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace Holism.DataAccess
{
    public abstract class DatabaseContext : DbContext
    {
        public abstract string ConnectionStringName { get; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Config.GetConnectionString(ConnectionStringName));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            foreach (var property in modelBuilder.Model.GetEntityTypes()
                                                .SelectMany(t => t.GetProperties())
                                                .Where(p => p.ClrType == typeof(Guid) && p.Name =="Guid")) 
            {
                    
                property.SetDefaultValueSql("newid()");
            }
            foreach (var entity in modelBuilder.Model.GetEntityTypes().Select(p => modelBuilder.Entity(p.ClrType)))
            {
                entity.Ignore("RelatedItems");
                Logger.LogInfo(entity.GetType().FullName);
            }
        }        
    }
}