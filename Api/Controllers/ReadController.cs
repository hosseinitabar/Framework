using Holism.Business;
using Holism.DataAccess;
using Holism.Models;
using Holism.Excel;
using Holism.Framework;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;

namespace Holism.Api.Controllers
{
    public abstract class ReadController<T> : DefaultController where T : class, IEntity, new()
    {
        public abstract ReadBusiness<T> ReadBusiness { get; }

        public static List<Action<T>> ItemAugmenters = new List<Action<T>>();

        public static List<Action<List<T>>> ListAugmenters = new List<Action<List<T>>>();

        public virtual bool HasHugeDataAndNeedsFilteringForExport { get { return false; } }

        public virtual Action<ListOptions> ListOptionsAugmenter { get; }

        public virtual Action<long> BeforeGet { get; }

        public virtual Action<T> BeforeReturningItem { get; }

        public virtual Action<List<T>> BeforeReturningList { get; }

        [HttpGet]
        public virtual ListResult<T> List([ModelBinder] ListOptions listOptions)
        {
            ListOptionsAugmenter?.Invoke(listOptions);
            var result = ReadBusiness.GetList(listOptions);
            ModifyListResultBeforeReturning(result);
            BeforeReturningList?.Invoke(result.Data);
            return result;
        }

        [HttpGet]
        public IActionResult Export([ModelBinder] ListOptions listOptions)
        {
            if (HasHugeDataAndNeedsFilteringForExport && !listOptions.HasFilters)
            {
                throw new BusinessException("لطفا ابتدا فیلتر اعمال کنید");
            }
            ListOptionsAugmenter?.Invoke(listOptions);
            listOptions.ReturnAll = true;
            var data = ReadBusiness.GetList(listOptions).Data;
            BeforeReturningList?.Invoke(data);

            var excelBytes = ExcelHelper.ListToExcel<T>(data);
            var fileName = typeof(T).Name + "Export.xlsx";
            return File(new MemoryStream(excelBytes), "application/octet-stream", fileName);
        }

        [HttpGet]
        public virtual T Get(long id)
        {
            BeforeGet?.Invoke(id);
            var item = ReadBusiness.Get(id);
            BeforeReturningItem?.Invoke(item);
            return item;
        }

        [HttpGet]
        public virtual T GetByGuid(Guid guid)
        {
            var item = ReadBusiness.Get(guid);
            BeforeReturningItem?.Invoke(item);
            return item;
        }

        protected virtual void ModifyListResultBeforeReturning(ListResult<T> items)
        {
            ModifyListBeforeReturning(items.Data);
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
    }
}