using System.Collections.Generic;

namespace Holism.DatabaseUpdater
{
    public class Table 
    {
        public string Name { get; set; }

        public bool HasGuid { get; set; }

        public bool IsEnum { get; set; }

        public List<Column> Columns { get; set; }

        public string OneToOneWith { get; set; }

        public List<ForeignKey> ForeignKeys { get; set; }

        public List<Index> Indexes { get; set; }
    }
}