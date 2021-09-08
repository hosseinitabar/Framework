namespace Holism.Generation
{
    public class Column
    {
        public string Name { get; set; }

        public string ClrType { get; set; }

        public string SqlType { get; set; }

        public bool IsNullable { get; set; }
        
        public bool IsComputed { get; set; }

        public string ComputedColumnFormula { get; set; }

        public bool HasDefault { get; set; }

        public string DefaultSqlText { get; set; }

        public bool IsIdentity { get; set; }

        public int MaxLength { get; set; }

        public int? Precision { get; set; }

        public int? Scale { get; set; }
    }
}