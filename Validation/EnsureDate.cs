using Holism.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Holism.Validation
{
    public class EnsureDate
    {
        DateTime? date;

        public EnsureDate(DateTime? date)
        {
            this.date = date;
        }

        public EnsureDate IsAfterThan(DateTime date)
        {
            if (this.date <= date)
            {
                throw new ClientException($"Ensure validation: '{this.date.ToString()}' should be after '{date.ToString()}'.");
            }
            return this;
        }

        public EnsureDate And()
        {
            return this;
        }
    }
}