using System.Collections.Generic;

namespace Holism.DatabaseUpdater
{
    public class View
    {
        public string Name { get; set; }

        public List<string> Query { get; set; }
    }
}