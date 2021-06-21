using Holism.Framework;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Holism.Api
{
    public class ListParametersModelBinder : IModelBinder
    {
        // todo: support this filtering: http://api.admin.vasak.local/charging/list?filters:["persianDate>=1398/09/23","persianDate<1398/09/24","color in gray,green,blue"]
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(ListParameters))
            {
                return Task.CompletedTask;
            }
            var valueProvider = bindingContext.ValueProvider;
            var listParameters = TryGetListParameters(valueProvider);
            if (listParameters == null)
            {
                listParameters = ListParameters.Create();
                TryGetPageSize(listParameters, valueProvider);
                TryGetPageNumber(listParameters, valueProvider);
                TryGetSorts(listParameters, valueProvider);
                TryGetSortRandomly(listParameters, valueProvider);
                TryGetFilters(listParameters, valueProvider);
                FallbackNullValues(listParameters);
                PreventAttack(listParameters);
            }
            EnsureNoDosAttackBecauseOfHugeVolumeOfDataCanOccure(listParameters);
            if (Framework.Config.IsDeveloping)
            {
                listParameters.RelatedItems.StartedAt = DateTime.Now;
            }
            bindingContext.Result = ModelBindingResult.Success(listParameters);
            return Task.CompletedTask;
        }

        private void EnsureNoDosAttackBecauseOfHugeVolumeOfDataCanOccure(ListParameters listParameters)
        {
            listParameters.ReturnAll = false;
        }

        private ListParameters TryGetListParameters(IValueProvider valueProvider)
        {
            var listParametersJson = valueProvider.GetValue("listParameters");
            if (listParametersJson != null && listParametersJson.Length > 0)
            {
                try
                {
                    var listParameters = listParametersJson.FirstValue.ToString().Deserialize<ListParameters>();
                    FallbackNullValues(listParameters);
                    PreventAttack(listParameters);
                    return listParameters;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        private void PreventAttack(ListParameters listParameters)
        {
            if (listParameters.PageSize > ListParameters.MaxClientPageSize)
            {
                var warning = $"We don't let page sizes greater than {ListParameters.MaxClientPageSize} by default. This should be determined explicitly by developers if necessary.";
                Logger.LogWarning(warning);
                listParameters.RelatedItems.Message = warning;
                listParameters.PageSize = ListParameters.MaxClientPageSize;
            }
            if (listParameters.ReturnAll == true)
            {
                var warning = "We don't let returning all items in a request. This is only available if developers explicitly determine it.";
                Logger.LogWarning(warning);
                listParameters.RelatedItems.Message = warning;
                listParameters.ReturnAll = false;
            }
        }

        private static void FallbackNullValues(ListParameters listParameters)
        {
            listParameters.PageNumber = listParameters.PageNumber ?? 1;
            listParameters.PageSize = listParameters.PageSize ?? Framework.Config.DefaultPageSize;
            listParameters.Sorts = listParameters.Sorts ?? new List<Sort>();
            listParameters.Filters = listParameters.Filters ?? new List<Filter>();
        }

        private static void TryGetFilters(ListParameters listParameters, IValueProvider valueProvider)
        {
            var filtersJson = valueProvider.GetValue("filters");
            if (filtersJson != null && filtersJson.Length > 0)
            {
                try
                {
                    listParameters.Filters = filtersJson.FirstValue.ToString().Deserialize<List<Filter>>();
                }
                catch (Exception)
                {
                    listParameters.Filters = new List<Filter>();
                }
            }
        }

        private void TryGetSorts(ListParameters listParameters, IValueProvider valueProvider)
        {
            var sortsJson = valueProvider.GetValue("sorts");
            if (sortsJson != null && sortsJson.Length > 0)
            {
                try
                {
                    listParameters.Sorts = sortsJson.FirstValue.ToString().Deserialize<List<Sort>>();
                }
                catch (Exception)
                {
                    listParameters.Sorts = new List<Sort>();
                }
            }
        }

        private static void TryGetSortRandomly(ListParameters listParameters, IValueProvider valueProvider)
        {
            if (listParameters.HasSorts)
            {
                return;
            }
            var sortRandomly = valueProvider.GetValue("sortRandomly");
            if (sortRandomly != null && sortRandomly.Length > 0)
            {
                listParameters.SortRandomly = sortRandomly.FirstValue.ToBoolean();
            }
        }

        private static void TryGetPageNumber(ListParameters listParameters, IValueProvider valueProvider)
        {
            var pageNumber = valueProvider.GetValue("pageNumber");
            if (pageNumber != null && pageNumber.Length > 0 && pageNumber.FirstValue.IsNumeric())
            {
                listParameters.PageNumber = (int)Convert.ToDecimal(pageNumber.FirstValue);
                if (listParameters.PageNumber < 1)
                {
                    listParameters.PageNumber = 1;
                }
            }
            else
            {
                listParameters.PageNumber = 1;
            }
        }

        private static void TryGetPageSize(ListParameters listParameters, IValueProvider valueProvider)
        {
            var pageSize = valueProvider.GetValue("pageSize");
            if (pageSize != null && pageSize.Length > 0 && pageSize.FirstValue.IsNumeric())
            {
                listParameters.PageSize = (int)Convert.ToDecimal(pageSize.FirstValue);
                if (listParameters.PageSize < 1)
                {
                    listParameters.PageSize = 1;
                }
            }
            else
            {
                listParameters.PageSize = Framework.Config.DefaultPageSize;
            }
        }
    }
}