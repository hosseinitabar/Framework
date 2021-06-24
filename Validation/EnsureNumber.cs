using Holism.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Holism.Validation
{
    public class EnsureNumber
    {
        decimal number;

        public EnsureNumber(decimal number)
        {
            this.number = number;
        }

        public EnsureNumber IsGreaterThanZero(string message = null)
        {
            if (number <= 0)
            {
                throw new ClientException(message ?? $"Ensure validation: {number} should be greater than zero.");
            }
            return this;
        }

        public EnsureNumber IsLessThan(int number, string message = null)
        {
            if (this.number >= number)
            {
                throw new ClientException(message ?? $"Ensure validation: {this.number} is not less than {number}.");
            }
            return this;
        }

        public EnsureNumber IsLessThanOrEqualTo(int number, string message = null)
        {
            if (this.number > number)
            {
                throw new ClientException(message ?? $"Ensure validation: {this.number} is not less than or equal to {number}");
            }
            return this;
        }

        public EnsureNumber And()
        {
            return this;
        }

        public EnsureNumber IsInteger()
        {
            int integer;
            if (!int.TryParse(number.ToString(), out integer))
            {
                throw new ClientException("Ensure validation: object is not an integer.");
            }
            return this;
        }

        public EnsureNumber CanBeCastTo<T>()
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ClientException($"{type.FullName} is not an enum");
            }
            var canBeCast = Enum.GetValues(type).Cast<int>().ToList().Contains((int)number);
            if (!canBeCast)
            {
                throw new ClientException($"{number} is not a valid member of {type.FullName}, thus can't be cast to it");
            }
            return this;
        }
    }
}