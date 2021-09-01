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
            ConfigGuid(modelBuilder);
            ConfigRelatedItems(modelBuilder);
        }        

        public void ConfigGuid(ModelBuilder modelBuilder) 
        {
            var guidProperties = modelBuilder.Model.GetEntityTypes()
                                                .SelectMany(t => t.GetProperties())
                                                .Where(p => p.ClrType == typeof(Guid) && p.Name =="Guid").ToList();
            foreach (var property in guidProperties)
            {
                property.SetDefaultValueSql("newid()");
            }
        }

        public void ConfigRelatedItems(ModelBuilder modelBuilder)
        {
            var entities = modelBuilder.Model.GetEntityTypes().Select(p => modelBuilder.Entity(p.ClrType));
            foreach (var entity in entities)
            {
                entity.Ignore("RelatedItems");
                Logger.LogInfo(entity.GetType().FullName);
            }
        }
    }
}