using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Holism.Framework;

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
            var allEntities = modelBuilder.Model.GetEntityTypes().Select(p => modelBuilder.Entity(p.ClrType));

            foreach (var entity in allEntities)
            {
                entity.Ignore("RelatedItems");
                var properties = entity.GetProperties();
                foreach (var property in properties)
                {
                    if (property.Name == "Guid") 
                    {
                        // set default to newid();
                        // set unique
                        // set type to be uniqueidentifier
                    }
                }
                // if it's Date, map it to datetime, and if it's DateOnly, map it to date and if it's type is Date but its name does not contain Date, throw error
            }
        }
    }
}