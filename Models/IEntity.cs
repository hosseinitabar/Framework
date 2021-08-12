using System;

namespace Holism.Models
{
    public interface IEntity
    {
        public IEntity()
        {
            RelatedItems = new System.Dynamic.ExpandoObject();
        }

        long Id { get; set; }

        dynamic RelatedItems { get; set; }
    }
}
