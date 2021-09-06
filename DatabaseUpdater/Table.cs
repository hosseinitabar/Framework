using System.Collections.Generic;

namespace Holism.DatabaseUpdater
{
    public class Table 
    {
        public string Name { get; set; }

        public List<Column> Column { get; set; }

        //public List<ForeignKeys> ForeignKeys { get; set; }

        //public List<Indexes> ForeignKeys { get; set; }
    }
}