using System.Collections.Generic;

namespace Holism.DatabaseUpdater
{
    public class Index
    {
        public string Name { get; set; }

        public List<string> Columns { get; set; }

        public string Filter { get; set; }
    }
}