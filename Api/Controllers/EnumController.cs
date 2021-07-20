using Holism.Business;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Holism.Api
{
    public abstract class EnumController<TEnum> : DefaultController where TEnum : struct, IConvertible
    {
        public abstract EnumBusiness<TEnum> EnumBusiness { get; }

        public EnumController()
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
        }

        [HttpGet]
        public List<EnumItem> All()
        {
            return EnumBusiness.GetAll();
        }
    }
}
