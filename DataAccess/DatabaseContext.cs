using Holism.Framework.Extensions;
using Holism.Normalization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Holism.DataAccess
{
    public class DatabaseContext : DbContext
    {
        public override int SaveChanges()
        {
            ApplySafePersianNormalization();

            ChangeTracker.AutoDetectChangesEnabled = false;
            var result = base.SaveChanges();
            ChangeTracker.AutoDetectChangesEnabled = true;

            return result;
        }

        private void ApplySafePersianNormalization()
        {
            ChangeTracker.DetectChanges();

            foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                SavePersianEncodeProperties(entry);
            }
        }

        private static void SavePersianEncodeProperties(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
        {
            foreach (var prop in entry.Properties)
            {
                if ((entry.State == EntityState.Added || prop.IsModified) && prop.OriginalValue is string)
                {
                    prop.CurrentValue = prop.CurrentValue.ToString().SafePersianEncode();
                }
            }
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ApplySafePersianNormalization();

            ChangeTracker.AutoDetectChangesEnabled = false;
            var result = base.SaveChanges(acceptAllChangesOnSuccess);
            ChangeTracker.AutoDetectChangesEnabled = true;

            return result;
        }
    }
}
