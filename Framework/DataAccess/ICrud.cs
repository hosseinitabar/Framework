using System;
using System.Collections.Generic;
using System.Text;
using Holism.Models;

namespace Holism.DataAccess
{
    public interface ICrud<T> : IRead<T> where T : IEntity, new()
    {
        void Delete(long id);

        T Update(T t);

        T Create(T t);

        T Upsert(T t);
    }
}