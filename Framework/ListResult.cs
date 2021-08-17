using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Holism.Framework
{
    public class ListResult<T>
    {
        public ListResult()
        {
            Data = new List<T>();
            RelatedItems = new ExpandoObject();
        }

        public List<T> Data { get; set; }

        public int? PageNumber { get; set; }

        public int? PageSize { get; set; }

        public long? From
        {
            get
            {
                if (!HasData)
                {
                    return null;
                }
                long? from = null;
                if (PageNumber.HasValue && PageSize.HasValue)
                {
                    from = (PageNumber - 1) * PageSize + 1;
                }
                if (TotalCount == null)
                {
                    return from;
                }
                if (from <= TotalCount)
                {
                    return from;
                }
                return null;
            }
        }

        public long? To
        {
            get
            {
                if (!HasData)
                {
                    return null;
                }
                long? to = null;
                if (From.HasValue)
                {
                    to = From + (long)PageSize - 1;
                }
                if (TotalCount == null)
                {
                    to = From + (long)Data.Count - 1;
                    return to;
                }
                if (to <= TotalCount)
                {
                    return to;
                }
                return TotalCount;
            }
        }

        public long? TotalCount { get; set; }

        public long? PagesCount
        {
            get
            {
                if (!HasData)
                {
                    return 0;
                }
                if (TotalCount == null)
                {
                    return null;
                }
                var pagesCount = (int)Math.Ceiling((decimal)TotalCount.Value / (decimal)PageSize);
                return pagesCount;
            }
        }

        public bool HasData
        {
            get
            {
                return Data.Count > 0;
            }
        }

        public bool? HasMore
        {
            get
            {
                if (!HasData)
                {
                    return false;
                }
                if (TotalCount.HasValue)
                {
                    if (To.HasValue)
                    {
                        if (To.Value == TotalCount.Value)
                        {
                            return false;
                        }
                        if (To.Value < TotalCount.Value)
                        {
                            return true;
                        }
                    }
                    return null;
                }
                if (TotalCount == null)
                {
                    if (To.HasValue && To < (From + PageSize))
                    {
                        return false;
                    }
                    return null;
                }
                return true;
            }
        }

        public ListResult<TOut> CopyFrom<TOut, TIn>(ListResult<TIn> source, Func<TIn, TOut> projector)
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

        public dynamic RelatedItems { get; set; }
    }
}
