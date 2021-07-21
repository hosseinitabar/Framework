using Holism.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Holism.Validation
{
    public class Ensure
    {
        object @object;

        public Ensure(object @object)
        {
            this.@object = @object;
        }

        public Ensure And()
        {
            return this;
        }

        public Ensure IsNotNull(string message = null)
        {
            if (@object == null)
            {
                throw new ClientException(message ?? "Ensure validation: object is null.");
            }
            return this;
        }

        public Ensure IsInteger()
        {
            int integer;
            if (!int.TryParse(@object.ToString(), out integer))
            {
                throw new ClientException("Ensure validation: object is not an integer.");
            }
            return this;
        }

        public EnsureString AsString()
        {
            if (@object == null)
            {
                return new EnsureString(null);
            }
            return new EnsureString(@object.ToString());
        }

        public EnsureGuid IsGuid(string message = null)
        {
            if (@object == null)
            {
                throw new ClientException(message ?? "GUID is null");
            }
            @object.ToString().Ensure().IsGuid();
            return new EnsureGuid(Guid.Parse(@object.ToString()));
        }

        public EnsureNumber IsNumeric(string message = null)
        {
            decimal number;
            string error = message ?? $"Ensure validation: object '{@object}' is not a valid number.";
            if (@object == null)
            {
                throw new ClientException(error);
            }
            if (!decimal.TryParse(@object.ToString(), out number))
            {
                throw new ClientException(error);
            }
            return new EnsureNumber(number);
        }

        public EnsureDate IsDate(string message = null)
        {
            DateTime date;
            if (!DateTime.TryParse(@object.ToString(), out date))
            {
                throw new ClientException(message ?? $"Ensure validation: '{@object}' is not a date");
            }
            return new EnsureDate(date);
        }

        public Ensure EqualsTo(object target, string message = null)
        {
            if (@object.ToString() == target.ToString())
            {
                return this;
            }
            throw new ClientException(message ?? "Values are not equal");
        }
    }
}