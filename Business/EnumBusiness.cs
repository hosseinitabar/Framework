using Holism.DataAccess;
using Holism.Framework;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Holism.Business
{
    public abstract class EnumBusiness<TEnum> where TEnum : struct, IConvertible
    {
        public abstract string ConnectionString { get; }

        private static Dictionary<string, List<EnumItem>> enumsDictionary;

        static EnumBusiness()
        {
            enumsDictionary = new Dictionary<string, List<EnumItem>>();
        }

        public EnumItem Get(long id)
        {
            string pluralName = GetPluralName();
            if (!enumsDictionary.ContainsKey(pluralName))
            {
                GetAll();
            }
            var enumItems = enumsDictionary[pluralName];
            var enumItem = enumItems.FirstOrDefault(i => i.Id == id);
            if (enumItem == null)
            {
                throw new ClientException($"There is no enum with Id {id} in {pluralName}");
            }
            return enumItem;
        }

        public EnumItem Get(Guid guid)
        {
            string pluralName = GetPluralName();
            if (!enumsDictionary.ContainsKey(pluralName))
            {
                GetAll();
            }
            var enumItems = enumsDictionary[pluralName];
            var enumItem = enumItems.FirstOrDefault(i => i.Guid == guid);
            if (enumItem == null)
            {
                throw new ClientException($"There is no enum with Guid {guid} in {pluralName}");
            }
            return enumItem;
        }

        public string GetKeyFromId(long id)
        {
            return Get(id).Key;
        }

        public string GetKeyFromGuid(Guid guid)
        {
            return Get(guid).Key;
        }

        public List<EnumItem> GetAll()
        {
            string pluralName = GetPluralName();
            if (enumsDictionary.ContainsKey(pluralName))
            {
                return enumsDictionary[pluralName];
            }
            List<EnumItem> enumItems = GetEnumItemsFromDatabase(pluralName);
            enumsDictionary.Add(pluralName, enumItems);
            return enumItems;
        }

        private static string GetPluralName()
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
            var name = typeof(TEnum).Name;
            var pluralName = name.Pluralize();
            return pluralName;
        }

        private List<EnumItem> GetEnumItemsFromDatabase(string pluralName)
        {
            var query = @$"
declare @query nvarchar(max) = '';
set @query = 
'select 
	Id,
	' +
	case
	    when (exists (select * from sys.columns where [object_id] = object_id('{pluralName}') and [name] = 'Guid')) then '[Guid]'
	    else 'Null'
	end
	+ ' as [Guid],'
	+ '
	Key,
from {pluralName}'
print @query
exec sp_executesql @query
";
            var items = Database.Open(ConnectionString).Get(query);
            var enumItems = new List<EnumItem>();
            foreach (DataRow row in items.Rows)
            {
                var enumItem = new EnumItem();
                enumItem.Id = row["Id"].ToString().ToInt();
                enumItem.Guid = row["Guid"].ToString().IsSomething() ? Guid.Parse(row["Guid"].ToString()) : Guid.Empty;
                enumItem.Key = row["Key"].ToString();
                if (enumItem.Id == 0 && enumItem.Key == "Unknown")
                {
                    continue;
                }
                enumItems.Add(enumItem);
            }
            return enumItems;
        }
    }
}