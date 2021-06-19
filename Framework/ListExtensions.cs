using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Holism.Framework
{
    public static class ListExtensions
    {
        public static T Random<T>(this List<T> list)
        {
            var randomItem = list.OrderBy(i => Guid.NewGuid()).FirstOrDefault();
            return randomItem;
        }

        public static List<T> Randomize<T>(this List<T> list)
        {
            var randomized = list.OrderBy(i => Guid.NewGuid()).ToList();
            return randomized;
        }

        public static string ToDelimitedSeparatedValues(this List<object> items, string delimiter)
        {
            var result = string.Join(delimiter, items.ToArray());
            return result;
        }

        public static string ToDelimitedSeparatedValues(this List<string> items, string delimiter)
        {
            var result = items.Select(i => (object)i).ToList().ToDelimitedSeparatedValues(delimiter);
            return result;
        }

        public static string ToCsv(this List<string> items)
        {
            return items.ToCsv(false);
        }

        public static string ToCsv(this List<string> items, bool hasSpaceAfterComma = false)
        {
            var comma = ",";
            if (hasSpaceAfterComma)
            {
                comma += " ";
            }
            var csv = items.Select(i => (object)i).ToList().ToDelimitedSeparatedValues(comma);
            csv = csv.Trim().TrimEnd(',').TrimStart(',').Trim();
            return csv;
        }

        public static string Merge(this List<string> items)
        {
            var merged = items.Select(i => (object)i).ToList().ToDelimitedSeparatedValues("\r\n");
            return merged;
        }

        public static IQueryable<T> RandomRecords<T>(this IQueryable<T> query, int count)
        {
            var result = query.OrderBy(i => Guid.NewGuid()).Take<T>(count);
            return result;
        }

        public static List<List<T>> GetCombinations<T>(this List<T> list)
        {
            var result = Enumerable
                .Range(1, (1 << list.Count) - 1)
                .Select(index => list.Where((item, idx) => ((1 << idx) & index) != 0).ToList())
                .ToList();
            return result;
        }

        public static string ToJavaScriptArray<T>(this List<T> list)
        {
            List<T> trimmedList = list.Select(i => (T)Convert.ChangeType(i.ToString().Trim(), typeof(T))).ToList<T>();
            bool isString = typeof(T) == typeof(string);
            string separator = isString ? "','" : ",";
            string start = isString ? "['" : "[";
            string end = isString ? "']" : "]";
            var result = $"{start}{string.Join(separator, trimmedList.ToArray<T>())}{end}";
            return result;
        }

        public static string ToCsv<T>(this List<T> list)
        {
            return list.ToCsv<T>(false);
        }

        public static string ToCsv<T>(this List<T> list, bool hasSpaceAfterComma = false)
        {
            var stringList = list.Select(i => i.ToString()).ToList();
            var result = stringList.ToCsv(hasSpaceAfterComma);
            return result;
        }

        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k)
        {
            var result = k == 0 ? new[] { new T[0] } : elements.SelectMany((e, i) => elements.Skip(i + 1).Combinations(k - 1).Select(c => (new[] { e }).Concat(c)));
            return result;
        }

        public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> list, int length)
        {
            if (length == 1)
            {
                var firstResult = list.Select(t => new T[] { t });
                return firstResult;
            }
            var result = Permutations(list, length - 1).SelectMany(t => list.Where(e => !t.Contains(e)), (t1, t2) => t1.Concat(new T[] { t2 }));
            return result;
        }

        public static DataTable ToTable<T>(this List<T> items)
        {
            var table = typeof(T).ToTableDefinition();
            if (items.IsNull() || items.Count == 0)
            {
                return table;
            }
            foreach (var item in items)
            {
                AddItemToTable(table, item);
            }
            return table;
        }

        private static void AddItemToTable<T>(DataTable table, T item)
        {
            var row = table.NewRow();
            var properties = typeof(T).GetProperties();
            var columns = row.Table.Columns;
            foreach (DataColumn column in columns)
            {
                var equivalentProperty = properties.SingleOrDefault(i => i.Name == column.ColumnName);
                if (equivalentProperty.IsNull())
                {
                    continue;
                }
                row[column] = equivalentProperty.GetValue(item) == null ? DBNull.Value : equivalentProperty.GetValue(item);
            }
            table.Rows.Add(row);
        }

        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
        {
            var result = source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
            return result;
        }
    }
}
