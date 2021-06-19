namespace Holism.DataAccess
{
    public interface IEntity
    {
        long Id { get; set; }

        dynamic RelatedItems { get; set; }
    }
}
