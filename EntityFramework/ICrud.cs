using System;
using System.Collections.Generic;
using System.Text;

namespace Holism.EntityFramework
{
    public interface ICrud<T> : IRead<T> where T : IEntity, new()
    {
        void Delete(long id);

        T Update(T t);

        T Create(T t);

        T Upsert(T t);
    }
}