using System.Linq.Dynamic.Core;
using Holism.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Holism.Framework;

namespace Holism.DataAccess
{
    public static class ListExtensions
    {
        public static ListResult<T> ApplyListParametersAndGetTotalCount<T>(this IQueryable<T> query, ListParameters listParameters)
        {
            ListParameters.ProvideDefaultValues(listParameters);
            var listResult = new ListResult<T>();
            var totalQueryable = FilterAndSort(query, listParameters.Filters, listParameters.Sorts, listParameters.SortRandomly);
            if (listParameters.ReturnAll)
            {
                listResult.Data = totalQueryable.ToList();
            }
            else
            {
                listResult.Data = totalQueryable.Skip<T>((listParameters.PageNumber.Value - 1) * listParameters.PageSize.Value).Take<T>(listParameters.PageSize.Value).ToList();
            }
            listResult.TotalCount = totalQueryable.Count();
            listResult.PageSize = listParameters.PageSize;
            listResult.PageNumber = listParameters.PageNumber;
            TransferMessages(listResult, listParameters);
            return listResult;
        }

        private static void TransferMessages<T>(ListResult<T> listResult, ListParameters listParameters)
        {
            foreach (KeyValuePair<string, object> item in listParameters.RelatedItems)
            {
                ExpandoObjectExtensions.AddProperty(listResult.RelatedItems, item.Key, item.Value);
            }
            if (Config.IsDeveloping)
            {
                listResult.RelatedItems.EndedAt = DateTime.Now;
                if (ExpandoObjectExtensions.Has(listParameters.RelatedItems, "StartedAt"))
                {
                    DateTime startedAt = listParameters.RelatedItems.StartedAt;
                    DateTime endedAt = listResult.RelatedItems.EndedAt;
                    listResult.RelatedItems.TimeInMilliseconds = endedAt.Subtract(startedAt).TotalMilliseconds;
                }
            }
        }

        public static ListResult<T> ApplyListParameters<T>(this IQueryable<T> query, ListParameters listParameters)
        {
            ListParameters.ProvideDefaultValues(listParameters);
            var listResult = new ListResult<T>();
            var totalQueryable = FilterAndSort(query, listParameters.Filters, listParameters.Sorts, listParameters.SortRandomly);
            if (listParameters.ReturnAll)
            {
                listResult.Data = totalQueryable.ToList();
            }
            else
            {
                listResult.Data = totalQueryable.Skip<T>((listParameters.PageNumber.Value - 1) * listParameters.PageSize.Value).Take<T>(listParameters.PageSize.Value).ToList();
            }
            listResult.PageSize = listParameters.PageSize;
            listResult.PageNumber = listParameters.PageNumber;
            TransferMessages(listResult, listParameters);
            return listResult;
        }

        public static int GetTotalCount<T>(this IQueryable<T> query, ListParameters listParameters)
        {
            ListParameters.ProvideDefaultValues(listParameters);
            var totalQueryable = FilterAndSort(query, listParameters.Filters, listParameters.Sorts, listParameters.SortRandomly);
            return totalQueryable.Count();
        }

        public static IQueryable<T> FilterAndSort<T>(this IEnumerable<T> query, List<Filter> filter, List<Sort> sort, bool sortRandomly)
        {
            return FilterAndSort(query.AsQueryable(), filter, sort, sortRandomly);
        }

        public static IQueryable<T> FilterAndSort<T>(this IQueryable<T> query, List<Filter> filters, List<Sort> sorts, bool sortRandomly)
        {
            query = ApplyFilters<T>(query, filters);
            if (sortRandomly)
            {
                query = query.OrderBy(i => Guid.NewGuid());
            }
            else
            {
                query = ApplySorts<T>(query, sorts);
            }
            return query;
        }

        private static IQueryable<T> ApplySorts<T>(IQueryable<T> query, List<Sort> sorts)
        {
            if (sorts.IsNull())
            {
                query = query.OrderBy("Id asc");
                return query;
            }
            sorts = sorts.Where(s => s.Property != null && s.Direction != null).ToList();
            if (sorts.Count == 0)
            {
                query = query.OrderBy("Id asc");
                return query;
            }
            string orderParams = "";
            sorts.ForEach(i => orderParams += i.Property + " " + i.Direction + ",");
            orderParams = orderParams.TrimEnd(',');
            query = query.OrderBy(orderParams);
            return query;
        }

        public static IQueryable<T> ApplyFilters<T>(IQueryable<T> items, List<Filter> filters)
        {
            if (filters == null)
                return items;
            filters = GetValidFilters(filters);
            NormalizeFilters(filters);
            TransformDateFilters(filters);

            foreach (var filter in filters)
            {
                items = ApplyFilter<T>(items, filter);
            }
            return items;
        }

        public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> items, Filter filter)
        {
            if (filter.Value == "null")
            {
                if (filter.Operator == FilterOperator.Equal)
                {
                    items = items.Where($"{filter.Property} = null");
                }
                if (filter.Operator == FilterOperator.NotEqual)
                {
                    items = items.Where($"{filter.Property} != null");
                }
                return items;
            }
            PropertyInfo propertyInfo = typeof(T).GetProperties().FirstOrDefault(i => i.Name.ToLower() == filter.Property.ToLower());
            if (propertyInfo.IsNull())
            {
                return items;
            }
            if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))
            {
                items = FilterDate<T>(items, filter);
            }
            else if (propertyInfo.PropertyType == typeof(string))
            {
                items = FilterString<T>(items, filter);
            }
            else if (propertyInfo.PropertyType.IsEnum)
            {
                items = FilterEnum<T>(items, propertyInfo, filter);
            }
            else if (propertyInfo.PropertyType == typeof(Boolean))
            {
                items = items.FilterBoolean<T>(filter);
            }
            else if (propertyInfo.PropertyType == typeof(Guid) || propertyInfo.PropertyType == typeof(Guid?))
            {
                items = items.FilterGuid<T>(filter);
            }
            else
            {
                if (filter.Property.Contains(' ') || filter.Value.Contains(' '))
                    throw new FrameworkException("Request is invalid");
                var filterClause = $"{filter.Property} {filter.OperatorMathematicalNotation} {filter.Value}";
                items = items.Where(filterClause);
            }
            return items;
        }

        private static IQueryable<T> FilterGuid<T>(this IQueryable<T> items, Filter filter)
        {
            if (filter.Operator != FilterOperator.Equal)
                throw new FrameworkException("Filter operator is not valid");
            Guid guid;
            if (!Guid.TryParse(filter.Value, out guid))
            {
                return items;
            }
            var filterClause = $"{filter.Property}.Equals(@0)";
            items = items.Where(filterClause, guid);
            return items;
        }

        private static IQueryable<T> FilterBoolean<T>(this IQueryable<T> items, Filter filter)
        {
            if (filter.Operator != FilterOperator.Equal)
                throw new FrameworkException("Filter operator is not valid");
            var filterClause = $"{filter.Property} = {filter.Value.ToBoolean()}";
            items = items.Where(filterClause);
            return items;
        }

        private static IQueryable<T> FilterEnum<T>(this IQueryable<T> items, PropertyInfo propertyInfo, Filter filter)
        {
            if (filter.Operator != FilterOperator.Equal)
                throw new FrameworkException("Filter operator is not valid");
            try
            {
                var value = Enum.Parse(propertyInfo.PropertyType, filter.Value);
                var filterClause = $"{filter.Property} = \"{value}\"";
                items = items.Where(filterClause);
                return items;
            }
            catch (Exception ex)
            {
                throw new FrameworkException($"{filter.Value} is not a valid value for {propertyInfo.PropertyType.Name}");
            }
        }

        private static IQueryable<T> FilterString<T>(IQueryable<T> items, Filter filter)
        {
            if (filter.Operator == FilterOperator.In)
            {
                return FilterStringForMultipleChoice(items, filter);
            }
            else
            {
                return FilterStringForOneChoice(items, filter);
            }
        }

        private static IQueryable<T> FilterStringForMultipleChoice<T>(IQueryable<T> items, Filter filter)
        {
            var whereCluase = string.Empty;
            if (filter.Property.EndsWith("Csv"))
            {
                filter.Values = filter.Values.Select(i => $",{i},").ToList();
            }
            for (int i = 0; i < filter.Values.Count; i++)
            {
                if (i > 0)
                {
                    whereCluase += " OR ";
                }
                whereCluase += (filter.Property + ".Contains(@" + i + ")");
            }
            Logger.LogInfo($"DynamicLinq: whereClause = '{whereCluase}' and parameter values = '{filter.Values.Merge()}'");
            items = items.Where(whereCluase, filter.Values.ToArray());
            return items;
        }

        private static IQueryable<T> FilterStringForOneChoice<T>(IQueryable<T> items, Filter filter)
        {
            var values = filter.Value.Split(' ');
            var whereCluase = string.Empty;
            for (int i = 0; i < values.Length; i++)
            {
                if (i > 0)
                {
                    whereCluase += " AND ";
                }
                if (filter.Operator == FilterOperator.Equal)
                    whereCluase += (filter.Property + ".Contains(@" + i + ")");
                else
                    whereCluase += (filter.Property + " != @" + i + "");
            }
            Logger.LogInfo($"DynamicLinq: whereClause = '{whereCluase}' and parameter values = '{values.ToList().Merge()}'");
            items = items.Where(whereCluase, values);
            return items;
        }

        private static IQueryable<T> FilterDate<T>(IQueryable<T> items, Filter filter)
        {
            DateTime date;
            if (DateTime.TryParse(filter.Value, out date))
            {
                items = items.Where(filter.Property + " " + filter.OperatorMathematicalNotation + " (@0)", date);
            }
            return items;
        }

        private static void NormalizeFilters(List<Filter> filters)
        {
            foreach (var filter in filters)
            {
                filter.Property = filter.Property.Trim();
                if (filter.Value.IsSomething())
                {
                    filter.Value = filter.Value.Trim();
                }
                if (filter.Values.IsNotNull())
                {
                    for (int i = 0; i < filter.Values.Count; i++)
                    {
                        filter.Values[i] = filter.Values[i].Trim();
                    }
                }
            }
        }

        private static List<Filter> GetValidFilters(List<Filter> filters)
        {
            // todo: how can we prevent any type of attach here?
            foreach (var filter in filters)
            {
                if (filter.Operator == FilterOperator.Unknown)
                    filter.Operator = FilterOperator.Equal;
            }
            var validFilters = filters.Where(i => IsValid(i)).ToList();
            return validFilters;
        }

        private static bool IsValid(Filter i)
        {
            if (i.Property.IsNothing())
            {
                return false;
            }
            if (i.Operator == FilterOperator.NotEqual)
            {
                return true;
            }
            if (i.Operator == FilterOperator.In && i.Values != null && i.Values.Count > 0)
            {
                return true;
            }
            var isValid = i.Value.IsSomething();
            return isValid;
        }
    }
}
