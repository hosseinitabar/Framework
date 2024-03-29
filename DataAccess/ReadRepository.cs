﻿using Holism.Framework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Holism.Models;

namespace Holism.DataAccess
{
    public class ReadRepository<T> : IRead<T> where T : class, IEntity, new()
    {
        protected readonly DbContext context;
        protected readonly DbSet<T> dbset;
        protected bool? SkipTotalCount;

        public ReadRepository(DbContext context)
        {
            this.context = context;
            // todo: can we force non-usage of sa user here? We might throw an exception complaining that user sa should be disabled, and another user should be used, for the sake of security through obscurity and security in depth.
            //this.context.Database.CommandTimeout = 2;
            this.dbset = context.Set<T>();
        }

        static ReadRepository()
        {
        }

        public IQueryable<T> GetList(Expression<Func<T, bool>> filter)
        {
            return All.Where(filter);
        }

        public virtual ListResult<T> GetList(ListParameters listParameters)
        {
            return GetList(All, listParameters);
        }

        public virtual ListResult<T> GetList(IQueryable<T> queryable, ListParameters listParameters)
        {
            try
            {
                if (SkipTotalCount == true)
                {
                    var result = queryable.ApplyListParameters(listParameters);
                    return result;
                }
                else
                {
                    var result = queryable.ApplyListParametersAndGetTotalCount(listParameters);
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogConnectionString();
                throw ex;
            }
        }

        public IQueryable<T> All
        {
            get
            {
                return dbset;
            }
        }

        public long FastCount
        {
            get
            {
                var count = Holism.DataAccess.Database.Open(ConnectionString).Get($@"
                    select sum(row_count)
                    from sys.dm_db_partition_stats
                    where [object_id] = object_id('{TableName}')
                    and (index_id = 0 or index_id = 1)
                ").Rows[0][0].ToString().ToLong();
                return count;
            }
        }

        public T Get(long id)
        {
            try
            {
                return dbset.Find(id);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                throw ex;
            }
        }

        public List<T> GetList(List<long> ids)
        {
            try
            {
                var items = dbset.Where(i => ids.Contains(i.Id)).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                throw ex;
            }
        }

        public T Get(Guid guid)
        {
            //EnsureIsGuidEntity();
            try
            {
                return dbset.Where($"@Guid.Equals(@0)", guid).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                throw ex;
            }
        }

        public List<T> GetList(List<Guid> guids)
        {
            //EnsureIsGuidEntity();
            try
            {
                var items = dbset.Where(i => guids.Contains(((IGuidEntity)i).Guid)).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                throw ex;
            }
        }

        private static void EnsureIsGuidEntity()
        {
            if (typeof(IGuidEntity).IsAssignableFrom(typeof(T)))
            {
                throw new ServerException($"Type {typeof(T).FullName} is not of type {typeof(IGuidEntity).FullName}. Thus a list of items based on a list of GUIDs can not be queried.");
            }
        }

        public bool Exists(Expression<Func<T, bool>> filter)
        {
            return Get(filter) != null;
        }

        public bool Exists(T t)
        {
            return GetIfExists(t) != null;
        }

        public bool Exists(long id)
        {
            return Get(id) != null;
        }

        public T Get(Expression<Func<T, bool>> filter)
        {
            try
            {
                return All.FirstOrDefault(filter);
            }
            catch (Exception ex)
            {
                LogConnectionString();
                throw ex;
            }
        }

        public virtual T GetIfExists(T t)
        {
            return Get(t.Id);
        }

        public List<T> Query(string query)
        {
            try
            {
                var result = dbset.FromSqlRaw<T>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                throw new ServerException($"Query method should return a record set that can be casted to type of {typeof(T).FullName}", ex);
            }
        }

        public ListResult<T> Query(string query, ListParameters listParameters)
        {
            try
            {
                if (SkipTotalCount == true)
                {
                    var result = dbset.FromSqlRaw<T>(query).ApplyListParameters(listParameters);
                    return result;
                }
                else
                {
                    var result = dbset.FromSqlRaw<T>(query).ApplyListParametersAndGetTotalCount(listParameters);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                throw new ServerException($"Query method should return a record set that can be casted to type of {typeof(T).FullName}");
            }
        }

        public DbContext Context
        {
            get { return context; }
        }

        public bool HasData
        {
            get
            {
                try
                {
                    bool hasData = All.FirstOrDefault() != null;
                    return hasData;
                }
                catch (Exception ex)
                {
                    LogConnectionString();
                    throw ex;
                }
            }
        }

        private void LogConnectionString()
        {
            Logger.LogError($"Error in connecting to {Context.Database.GetDbConnection().ConnectionString}: ");
        }

        public T Random()
        {
            try
            {
                var result = All.OrderBy(i => Guid.NewGuid()).FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                LogConnectionString();
                throw ex;
            }
        }

        public T Random(Expression<Func<T, bool>> filter)
        {
            try
            {
                var result = All.Where(filter).OrderBy(i => Guid.NewGuid()).FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                LogConnectionString();
                throw ex;
            }
        }

        public string ConnectionString
        {
            get
            {
                return context.Database.GetDbConnection().ConnectionString;
            }
        }

        public string Database
        {
            get
            {
                return context.Database.GetDbConnection().Database;
            }
        }

        public virtual string TypeName
        {
            get
            {
                return typeof(T).Name;
            }
        }

        public virtual string TableName
        {
            get
            {
                throw new ServerException("Override TableName property");
            }
        }
    }
}