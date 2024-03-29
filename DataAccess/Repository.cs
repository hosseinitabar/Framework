﻿using EFCore.BulkExtensions;
using Holism.Framework;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Transactions;
using Holism.Models;

namespace Holism.DataAccess
{
    public class Repository<T> : ReadRepository<T>, ICrud<T> where T : class, IEntity, new()
    {
        static object lockToken = new object();

        public Repository(DbContext dbContext)
            : base(dbContext)
        {
        }

        public virtual void Delete(long id)
        {
            var entity = Get(id);
            if (entity == null)
            {
                return;
            }
            var entry = context.Entry(entity);
            entry.State = EntityState.Deleted;
            context.SaveChanges();
        }

        public virtual void Delete(T t)
        {
            if (t == null)
            {
                return;
            }
            var entity = GetIfExists(t);
            if (entity != null)
            {
                var entry = context.Entry(entity);
                entry.State = EntityState.Deleted;
                context.SaveChanges();
            }
        }

        public virtual T Create(T t)
        {
            if (t == null)
            {
                throw new ServerException($"{typeof(T).Name} is null");
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
            dbset.Attach(t);
            context.Entry(t).State = EntityState.Modified;
            context.SaveChanges();
            return t;
        }

        public T Upsert(T t)
        {
            T _temp = Get(t.Id);
            if (_temp == null)
            {
                if (t.Id > 0)
                {
                    throw new ServerException("Trying to upsert an item that doesn't exist, and providing an Id for it have conflic with each other. This scenario might happen in one-to-one relationships. Please drop the Id for the item to be created, or change the Id and make sure that it already exists.");
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
            Holism.DataAccess.Database.Open(context.Database.GetDbConnection().ConnectionString).Run($"alter table {TableName} with check check constraint all");
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
                Logger.LogException(ex);
                Logger.LogError($"Error running query on database:\r\n{query}");
                throw new ServerException($"Error running query on database:");
            }
        }
    }
}
