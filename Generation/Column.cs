namespace Holism.Generation
{
    public class Column
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string DotNetType
        {
            get
            {
                if (Name.EndsWith("Guid"))
                {
                    return "Guid";
                }
                if (Name.EndsWith("Id"))
                {
                    return "long";
                }
                if (Name.Contains("Date"))
                {
                    return "DateTime";
                }
                if (Type == "int")
                {
                    return "int";
                }
                if (Type == null)
                {
                    return "object";
                }
                return "string";
            }
        }

        public string SqlType
        {
            get
            {
                if (Name.EndsWith("Guid"))
                {
                    return "uniqueidentifier";
                }
                if (Name.EndsWith("Id"))
                {
                    return "bigint";
                }
                if (Name.Contains("Date"))
                {
                    return "datetime";
                }
                if (Type == "int")
                {
                    return "int";
                }
                if (Type == "long")
                {
                    return "bigint";
                }
                if (Type == "decimal")
                {
                    return "decimal";
                }
                return "nvarchar(400)";
            }
        }

        public bool IsNullable { get; set; }
        
        public bool HasDefault { get; set; }

        public string DefaultSqlText { get; set; }

        public int? Precision { get; set; }

        public int? Scale { get; set; }
    }
}