using System;
using System.Collections.Generic;
using System.Text;

namespace Holism.Framework
{
    public class ClientException : Exception
    {
        public string Code { get; set; }

        public new object Data { get; set; }

        public ClientException(string message)
            : base(message)
        {

        }

        public ClientException(string message, string code)
            : base(message)
        {
            Code = code;
        }

        public ClientException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public ClientException(string message, string code, Exception innerException)
            : base(message, innerException)
        {
            Code = code;
        }

        public ClientException(string message, string code, object data)
            : base(message)
        {
            Code = code;
            Data = data;
        }

        public ClientException(string message, string code, object data, Exception innerException)
            : base(message, innerException)
        {
            Code = code;
            Data = data;
        }
    }
}