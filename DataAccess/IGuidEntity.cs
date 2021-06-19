using System;

namespace Holism.DataAccess
{
    public interface IGuidEntity : IEntity
    {
        Guid Guid { get; set; }
    }
}
