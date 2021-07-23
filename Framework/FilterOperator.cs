using System.Runtime.Serialization;

namespace Holism.Framework
{
    public enum FilterOperator
    {
        Unknown = 0,
        [EnumMember(Value = "=")]
        Equal = 1,
        [EnumMember(Value = ">")]
        GreaterThan = 2,
        [EnumMember(Value = "<")]
        LessThan = 3,
        [EnumMember(Value = ">=")]
        GreaterThanOrEqual = 4,
        [EnumMember(Value = "<=")]
        LessThanOrEqual = 5,
        [EnumMember(Value = "!=")]
        NotEqual = 6,
        In = 7,
        NotIn = 8
    }
}
