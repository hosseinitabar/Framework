using Holism.Framework;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Holism.Framework
{
    public class ListParameters
    {
        public const int MaxClientPageSize = 100;

        private ListParameters()
        {
            RelatedItems = new ExpandoObject();
        }

        public List<Sort> Sorts { get; set; }

        public bool SortRandomly { get; set; }

        public List<Filter> Filters { get; set; }

        public int? PageNumber { get; set; }

        public int? PageSize { get; set; }

        public bool ReturnAll { get; set; }

        public bool HasSorts
        {
            get
            {
                return Sorts.Count > 0;
            }
        }

        public bool HasFilters
        {
            get
            {
                return Filters.Count > 0;
            }
        }

        public void AddFilter(Filter filter)
        {
            if (Filters == null)
            {
                Filters = new List<Filter>();
            }
            Filters.Add(filter);
        }

        public Filter GetFilter(string property)
        {
            var filter = Filters.FirstOrDefault(i => i.Property.ToLower() == property.ToLower());
            return filter;
        }

        public void AddFilter<T>(Expression<Func<T, object>> propertySelector, string value, FilterOperator filterOperator = FilterOperator.Equal)
        {
            string property = GetPropertyName(propertySelector);
            AddFilter(new Filter
            {
                Property = property,
                Value = value,
                Operator = filterOperator
            });
        }

        public Filter GetFilter<T>(Expression<Func<T, object>> propertySelector)
        {
            string property = GetPropertyName(propertySelector);
            var filter = Filters.Find(i => i.Property.ToLower() == property.ToLower());
            return filter;
        }

        private static string GetPropertyName<T>(Expression<Func<T, object>> propertySelector)
        {
            string property;
            if (propertySelector.Body is MemberExpression)
            {
                property = ((MemberExpression)propertySelector.Body).Member.Name;
            }
            else
            {
                var operand = ((UnaryExpression)propertySelector.Body).Operand;
                property = ((MemberExpression)operand).Member.Name;
            }
            return property;
        }

        public bool HasFilter<T>(Expression<Func<T, object>> propertySelector)
        {
            string property = GetPropertyName(propertySelector);
            return HasFilter(property);
        }

        public bool HasFilter(string property)
        {
            var hasFilter = Filters.Any(i => i.Property.ToLower() == property.ToLower());
            return hasFilter;
        }

        public void RemoveFilter(Filter filter)
        {
            if (!HasFilter(filter.Property))
            {
                return;
            }
            filter = GetFilter(filter.Property);
            Filters.Remove(filter);
        }

        public void ChangeFilterProperty(Filter filter, string newPropertyName)
        {
            filter = GetFilter(filter.Property);
            filter.Property = newPropertyName;
        }

        public void AddSort(Sort sort)
        {
            if (Sorts == null)
            {
                Sorts = new List<Sort>();
            }
            Sorts.Add(sort);
        }

        private void AddSort<T>(Expression<Func<T, object>> propertySelector, string direction)
        {
            string property;
            if (propertySelector.Body is MemberExpression)
            {
                property = ((MemberExpression)propertySelector.Body).Member.Name;
            }
            else
            {
                var operand = ((UnaryExpression)propertySelector.Body).Operand;
                property = ((MemberExpression)operand).Member.Name;
            }
            AddSort(new Sort
            {
                Property = property,
                Direction = direction
            });
        }

        public void AddSort<T>(Expression<Func<T, object>> propertySelector, SortDirection direction)
        {
            if (direction == SortDirection.Unknown)
            {
                return;
            }
            string sort = "asc";
            if (direction == SortDirection.Descending)
            {
                sort = "desc";
            }
            AddSort<T>(propertySelector, sort);
        }

        public static ListParameters Create()
        {
            var listParameters = new ListParameters();
            listParameters.PageSize = Config.DefaultPageSize;
            listParameters.PageNumber = 1;
            listParameters.Filters = new List<Filter>();
            listParameters.Sorts = new List<Sort>();
            return listParameters;
        }

        public static ListParameters Create(int? pageNumber, int? pageSize)
        {
            var listParameters = Create();
            if (pageNumber.HasValue)
            {
                listParameters.PageNumber = pageNumber;
            }
            if (pageSize.HasValue)
            {
                if (pageSize > MaxClientPageSize)
                {
                    pageSize = MaxClientPageSize;
                }
                listParameters.PageSize = pageSize;
            }
            return listParameters;
        }

        public static void ProvideDefaultValues(ListParameters listParameters)
        {
            if (listParameters == null)
            {
                listParameters = ListParameters.Create();
            }
            listParameters.PageNumber = listParameters.PageNumber ?? 1;
            if (listParameters.PageNumber < 1)
            {
                listParameters.PageNumber = 1;
            }
            listParameters.PageSize = listParameters.PageSize ?? 10; //  Config.DefaultPageSize; todo
            if (listParameters.Filters == null)
            {
                listParameters.Filters = new List<Filter>();
            }
            if (listParameters.Sorts == null)
            {
                listParameters.Sorts = new List<Sort>();
            }
        }

        public dynamic RelatedItems { get; set; }
    }
}
