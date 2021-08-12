using System.Collections.Generic;

namespace Holism.Framework
{
    public class Filter
    {
        public string Property { get; set; }

        public FilterOperator Operator { get; set; }

        public string OperatorMathematicalNotation
        {
            get
            {
                return FilterOperatorNormalizer.NormalizeFilterOperator(Operator);
            }
        }

        public object Value { get; set; }

        public List<string> Values { get; set; }
    }
}
