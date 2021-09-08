namespace Holism.DatabaseUpdater
{
    public class ForeignKey
    {
        public string ReferencingTable { get; set; }

        public string ReferencingColumn { get; set; }

        public string ReferencedTable { get; set; }

        public string ReferencedColumn { get; set; }
    }
}