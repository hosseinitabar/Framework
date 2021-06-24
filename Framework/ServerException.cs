using System;
using System.Collections.Generic;
using System.Text;

namespace Holism.Framework
{
    public class ServerException : Exception
    {
        public ServerException(string message)
            : base(message)
        {

        }

        public ServerException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}