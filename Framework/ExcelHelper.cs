using ClosedXML.Excel;
using Holism.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Holism.Framework
{
    public class ExcelHelper
    {
        //public static byte[] ListToExcel<T>(List<T> items)
        //{
        //    var table = items.ToTable<T>();
        //    var workbook = new XLWorkbook();
        //    workbook.Worksheets.Add(table, "Export");
        //    var result = new MemoryStream();
        //    workbook.SaveAs(result);
        //    result.Position = 0;
        //    return result.GetBytes();
        //}

        public static byte[] ListToExcel<T>(List<T> items)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Export");
            var properties = typeof(T).GetProperties().Where(i => i.Name != "RelatedItems").ToList();
            for (int i = 0; i < properties.Count; i++)
            {
                worksheet.Cell(1, i + 1).Value = properties[i].Name;
            }
            for (int row = 0; row < items.Count; row++)
            {
                for (int column = 0; column < properties.Count; column++)
                {
                    worksheet.Cell(row + 2, column + 1).Value = properties[column].GetValue(items[row]);
                }
            }
            var result = new MemoryStream();
            workbook.SaveAs(result);
            result.Position = 0;
            return result.GetBytes();
        }
    }
}
