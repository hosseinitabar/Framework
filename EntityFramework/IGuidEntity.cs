using System;

namespace Holism.EntityFramework
{
    public interface IGuidEntity : IEntity
    {
        Guid Guid { get; set; }
    }
}
