using System.Collections.Generic;

namespace Holism.Generation
{
    public class Database
    {
        public string Name { get; set; }

        public List<Table> Tables { get; set; }

        public List<View> Views { get; set; }
    }
}