using Holism.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Holism.Models;

namespace Holism.DataAccess
{
    public interface IRead<T> where T : IEntity, new()
    {
        IQueryable<T> GetList(Expression<Func<T, bool>> filter);

        ListResult<T> GetList(ListParameters listParameters);

        IQueryable<T> All { get; }

        T Get(long id);

        T Get(Expression<Func<T, bool>> filter);

        T GetIfExists(T t);

        bool Exists(Expression<Func<T, bool>> filter);

        bool Exists(T t);
    }
}
