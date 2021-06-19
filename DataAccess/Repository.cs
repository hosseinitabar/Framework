using EFCore.BulkExtensions;
using Holism.Framework;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Transactions;

namespace Holism.DataAccess
{
    public abstract class Repository<T> : ReadRepository<T>, ICrud<T> where T : class, IEntity, new()
    {
        static object lockToken = new object();

        public Repository(DbContext dbContext)
            : base(dbContext)
        {
        }

        public void OnBufferFlushError(Action<object[]> handler)
        {
            lock (lockToken)
            {
                BufferManager.RegisterBufferFlushErrorHandler(BufferKey(), handler);
            }
        }

        public virtual void Delete(long id)
        {
            var entity = Get(id);
            if (entity.IsNull())
            {
                return;
            }
            var entry = context.Entry(entity);
            entry.State = EntityState.Deleted;
            context.SaveChanges();
        }

        public virtual void Delete(T t)
        {
            if (t.IsNull())
            {
                return;
            }
            var entity = GetIfExists(t);
            if (entity.IsNotNull())
            {
                var entry = context.Entry(entity);
                entry.State = EntityState.Deleted;
                context.SaveChanges();
            }
        }

        public virtual void BufferToFlush(T t)
        {
            lock (lockToken)
            {
                BufferManager.AddToBuffer(BufferKey(), t, FlushBuffer);
            }
        }

        public void FlushBuffer()
        {
            var entities = BufferManager.Get(BufferKey()).Select(i => (T)i).ToList();
            if (entities.Count == 0)
            {
                return;
            }

            BulkInsert(entities);

            BufferManager.Empty(BufferKey());
        }

        public string BufferKey()
        {
            return typeof(T).FullName;
        }

        public virtual T Create(T t)
        {
            if (t.IsNull())
            {
                throw new FrameworkException($"{typeof(T).Name} is null");
            }
            try
            {
                dbset.Add(t);
                context.SaveChanges();
            }
            //catch (System.Data.Entity.Core.EntityException ex)
            //{
            //    Logger.LogError("Connection string: " + context.Database.GetDbConnection().ConnectionString);
            //    if (Framework.ExceptionHelper.BuildExceptionString(ex).Contains("The underlying provider failed on Open."))
            //    {
            //        Logger.LogError("Can't connect to database. Please check app.config or web.config exists, and it's configured property in both Settings and ConnectionStrings elements (probably using external configurations). Also make sure DbContext files use Config.DatabaseName to call their base constructors. At last make sure that database name in Settings.config is OK and it exists in ConnectionStrings.config.");
            //    }
            //    throw ex;
            //}
            catch (DbUpdateException ex)
            {
                Logger.LogError("Connection string: " + context.Database.GetDbConnection().ConnectionString);
                SqlExceptionHelper.HandleSqlException(ex, TypeName);
            }
            return t;
        }

        public virtual T Update(T t)
        {
            return Update(Database, t);
        }

        public virtual T Update(string databaseName, T t)
        {
            using (var _context = (Activator.CreateInstance(context.GetType(), databaseName) as DbContext))
            {
                DbSet<T> _dbset = _context.Set<T>();
                _dbset.Attach(t);
                _context.Entry(t).State = EntityState.Modified;
                _context.SaveChanges();
            }
            return t;
        }

        public T Upsert(T t)
        {
            T _temp = Get(ExistenceFilter(t));
            if (_temp.IsNull())
            {
                if (t.Id > 0)
                {
                    throw new FrameworkException("Trying to upsert an item that doesn't exist, and providing an Id for it have conflic with each other. This scenario might happen in one-to-one relationships. Please drop the Id for the item to be created, or change the Id and make sure that it already exists.");
                }
                var model = Create(t);
                dynamic relatedItems = new ExpandoObject();
                relatedItems.IsCreation = true;
                model.GetType().GetProperty("RelatedItems").SetValue(t, relatedItems);
                return model;
            }
            else
            {
                _temp = _temp.GetNewValues<T>(t, "Id");
                return Update(_temp);
            }
        }

        public void BulkInsert(List<T> entities)
        {
            if (entities.Count == 0)
            {
                return;
            }
            context.BulkInsert(entities);
        }

        public void BulkUpdate(List<T> entities)
        {
            if (entities.Count == 0)
            {
                return;
            }
            context.BulkUpdate(entities);
        }

        private void RestoreConstratints()
        {
            if (Config.HasSetting("IgnoreRestoringConstraintsAfterBulkInsert"))
            {
                return;
            }
            Sql.Database.Open(context.Database.GetDbConnection().ConnectionString).Run($"alter table {TableName} with check check constraint all");
        }

        public void BulkDelete(List<T> items)
        {
            context.BulkDelete(items);
        }

        public void Run(string query)
        {
            try
            {
                query += $"\r\n select top 1 * from {TableName}";
                var result = dbset.FromSqlRaw<T>(query).ToList();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                Logger.LogError($"Error running query on database:\r\n{query}");
                throw new FrameworkException($"Error running query on database:");
            }
        }
    }
}
