using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Holism.Generation
{
    public class UniqueKey
    {
        public string Name { get; set; }

        public List<Column> Columns { get; set; }

        public Table Table { get; set; }
    }
}