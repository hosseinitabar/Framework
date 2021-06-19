using System;

namespace Holism.Models
{
    public interface IGuidEntity : IEntity
    {
        Guid Guid { get; set; }
    }
}
