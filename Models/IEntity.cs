namespace Holism.Models
{
    public interface IEntity
    {
        long Id { get; set; }

        dynamic RelatedItems { get; set; }
    }
}
