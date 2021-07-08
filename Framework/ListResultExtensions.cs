using System;
using System.Collections.Generic;
using System.Text;

namespace Holism.Framework
{
    public static class ListResultExtensions
    {
        public static ListResult<TOut> Convert<TIn, TOut>(this ListResult<TIn> source, Func<TIn, TOut> projector)
        {
            var target = new ListResult<TOut>();
            target.PageNumber = source.PageNumber;
            target.PageSize = source.PageSize;
            target.TotalCount = source.TotalCount;
            target.Data = new List<TOut>();
            foreach (var item in source.Data)
            {
                target.Data.Add(projector(item));
            }
            return target;
        }

        public static ListResult<TOut> BulkConvert<TIn, TOut>(this ListResult<TIn> source, Func<List<TIn>, List<TOut>> projector)
        {
            var target = new ListResult<TOut>();
            target.PageNumber = source.PageNumber;
            target.PageSize = source.PageSize;
            target.TotalCount = source.TotalCount;
            target.Data = projector(source.Data);
            return target;
        }
    }
}
