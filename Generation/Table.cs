using System.Collections.Generic;
using Humanizer;
using Humanizer.Inflections;
using System.Linq;

namespace Holism.Generation
{
    public class Table 
    {
        public int ObjectId { get; set; }

        public string Name { get; set; }

        public string SingularName
        {
            get
            {
                return Name.Singularize();
            }
        }

        public string PluralName
        {
            get
            {
                return Name.Pluralize();
            }
        }

        public string SqlQualifiedName
        {
            get
            {
                return $"[{PluralName}]";
            }
        }

        public bool UsesNts
        {
            get
            {
                var sqlServerSpatialDataTypes = new List<string> { "geometry", "geography" };
                if (Columns.Any(i => sqlServerSpatialDataTypes.Contains(i.SqlType)))
                {
                    return true;
                }
                return false;
            }
        }

        public bool HasGuid { get; set; }

        public bool IsEnum { get; set; }

        public List<Column> Columns { get; set; }

        public string OneToOneWith { get; set; }

        public List<ForeignKey> ForeignKeys { get; set; }

        public List<Index> Indexes { get; set; }

        public string GeneratedCode { get; set; }

        public List<UniqueKey> UniqueKeys { get; set; }
        
        public bool IsView { get; set; }

        public bool HasUniqueKeys
        {
            get
            {
                return UniqueKeys.Count > 0;
            }
        }
    }
}