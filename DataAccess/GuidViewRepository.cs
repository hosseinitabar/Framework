using Holism.Framework;
using Holism.Framework.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Holism.DataAccess
{
    public class GuidViewRepository<T> : ViewRepository<T> where T : class, IGuidEntity, new()
    {
        public GuidViewRepository(DbContext context)
            : base(context)
        {
        }

        public T Get(Guid guid)
        {
            try
            {
                var entity = dbset.FirstOrDefault(i => i.Guid == guid);
                return entity;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                throw ex;
            }
        }

        public List<T> GetList(List<Guid> guids)
        {
            try
            {
                var items = dbset.Where(i => guids.Contains(i.Guid)).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                throw ex;
            }
        }

        public bool Exists(Guid guid)
        {
            return Get(guid).IsNotNull();
        }
    }
}
