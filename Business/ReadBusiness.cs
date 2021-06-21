using Holism.Models;
using Holism.DataAccess;
using Holism.Framework;
using Holism.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Holism.Business
{
    public abstract class ReadBusiness<T> where T : class, IEntity, new()
    {
        public static object lockToken = new object();

        protected abstract ReadRepository<T> ReadRepository { get; }

        protected virtual Expression<Func<T, object>> DefaultDescendingSortProperty { get; }

        public static List<Action<T>> ItemAugmenters = new List<Action<T>>();

        public static List<Action<List<T>>> ListAugmenters = new List<Action<List<T>>>();

        public virtual ListResult<T> GetList(ListParameters listParameters)
        {
            return GetList(ReadRepository.All, listParameters);
        }

        public virtual ListResult<T> GetList(IQueryable<T> queryable, ListParameters listParameters)
        {
            if (!listParameters.HasSorts && DefaultDescendingSortProperty != null)
            {
                listParameters.AddSort<T>(DefaultDescendingSortProperty, SortDirection.Descending);
            }
            var items = ReadRepository.GetList(queryable, listParameters);
            ModifyListResultBeforeReturning(items);
            if (items.Data.Count > 100)
            {
                items.RelatedItems.Warning = "Please consider using page sizes lesser than 100 to reduce the network latency, and performance issues";
            }
            return items;
        }

        public virtual List<T> GetList(List<long> ids)
        {
            var items = ReadRepository.GetList(ids);
            ModifyListBeforeReturning(items);
            return items;
        }

        public virtual List<T> GetList(List<Guid> guids)
        {
            var items = ReadRepository.GetList(guids);
            ModifyListBeforeReturning(items);
            return items;
        }

        protected virtual List<T> GetList(Expression<Func<T, bool>> filter)
        {
            var items = ReadRepository.GetList(filter).ToList();
            ModifyListBeforeReturning(items);
            return items;
        }

        public virtual List<T> GetRandomList(int count)
        {
            var items = ReadRepository.All.OrderBy(i => Guid.NewGuid()).Take(count).ToList();
            ModifyListBeforeReturning(items);
            return items;
        }

        public virtual List<T> Query(string query)
        {
            var items = ReadRepository.Query(query);
            ModifyListBeforeReturning(items);
            return items;
        }

        protected virtual void ModifyListResultBeforeReturning(ListResult<T> items)
        {
            ModifyListBeforeReturning(items.Data);
        }

        public virtual List<T> GetAll()
        {
            var items = ReadRepository.All.ToList();
            ModifyListBeforeReturning(items);
            return items;
        }

        public virtual List<T> GetAllAtRandom()
        {
            var items = ReadRepository.All.ToList().OrderBy(i => Guid.NewGuid()).ToList();
            ModifyListBeforeReturning(items);
            return items;
        }

        public virtual List<T> GetAllWithoutModifications()
        {
            return ReadRepository.All.ToList();
        }

        protected virtual void ModifyListBeforeReturning(List<T> items)
        {
            foreach (var listAugmenter in ListAugmenters)
            {
                listAugmenter.Invoke(items);
            }
            foreach (var item in items)
            {
                ModifyItemBeforeReturning(item);
            }
        }

        public virtual T Get(long id)
        {
            if (id == 0)
            {
                throw new ClientException($"Id of {typeof(T).Name} must be greater than zero.");
            }
            var item = ReadRepository.Get(id);
            if (item == null)
            {
                Logger.LogWarning($"Type {typeof(T).FullName} with id {id} does not exist");
                throw new ClientException($"{typeof(T).Name} {id} does not exist");
            }
            ModifyItemBeforeReturning(item);
            return item;
        }

        public virtual T Get(Guid guid)
        {
            guid.ToString().Ensure().AsString().IsNotEmptyGuid();
            var item = ReadRepository.Get(guid);
            if (item == null)
            {
                Logger.LogWarning($"Type {typeof(T).FullName} with GUID {guid} does not exist");
                throw new ClientException($"{typeof(T).Name} {guid} does not exist");
            }
            ModifyItemBeforeReturning(item);
            return item;
        }

        protected virtual T Get(Expression<Func<T, bool>> filter)
        {
            var item = ReadRepository.Get(filter);
            if (item == null)
            {
                Logger.LogWarning($"Type {typeof(T).FullName} with filter {filter.ToString()} does not exist");
                throw new ClientException("item does not exist");
            }
            ModifyItemBeforeReturning(item);
            return item;
        }

        public virtual T GetOrNull(long id)
        {
            var item = ReadRepository.Get(id);
            if (item != null)
            {
                ModifyItemBeforeReturning(item);
            }
            return item;
        }

        protected virtual T GetOrNull(Expression<Func<T, bool>> filter)
        {
            var item = ReadRepository.Get(filter);
            if (item != null)
            {
                ModifyItemBeforeReturning(item);
            }
            return item;
        }

        public virtual T GetRandom()
        {
            var item = ReadRepository.All.OrderBy(i => Guid.NewGuid()).FirstOrDefault();
            if (item != null)
            {
                ModifyItemBeforeReturning(item);
            }
            return item;
        }

        protected virtual void ModifyItemBeforeReturning(T item)
        {
            if (item == null)
            {
                return;
            }
            foreach (var itemAugmenter in ItemAugmenters)
            {
                itemAugmenter.Invoke(item);
            }
        }

        public virtual bool Exists(long id)
        {
            return ReadRepository.Exists(id);
        }

        protected virtual bool Exists(Expression<Func<T, bool>> filter)
        {
            return ReadRepository.Exists(filter);
        }

        public virtual T Random()
        {
            var item = ReadRepository.Random();
            if (item != null)
            {
                ModifyItemBeforeReturning(item);
            }
            return item;
        }

        public virtual T Random(Expression<Func<T, bool>> filter)
        {
            var item = ReadRepository.Random(filter);
            if (item != null)
            {
                ModifyItemBeforeReturning(item);
            }
            return item;
        }
    }
}