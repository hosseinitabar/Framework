using System;
using System.Collections.Generic;
using System.Text;

namespace Holism.Api
{
    public class DebuggingInfo
    {
        public string Controller { get; set; }

        public string Action { get; set; }

        public string TypeFqn { get; set; }

        public string Route { get; set; }
    }
}
