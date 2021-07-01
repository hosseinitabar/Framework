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
            }
        }
    }
}