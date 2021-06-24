using Holism.Framework;
using System;

namespace Holism.Validation
{
    public class EnsureGuid
    {
        Guid guid;

        public EnsureGuid(Guid guid)
        {
            this.guid = guid;
        }

        public EnsureGuid IsNotEmpty(string message = null)
        {
            if (guid == Guid.Empty)
            {
                throw new ClientException(message ?? "GUID is empty");
            }
            return this;
        }

        public EnsureGuid And()
        {
            return this;
        }
    }
}
