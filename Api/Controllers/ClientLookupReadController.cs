using Holism.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Holism.Api.Controllers
{
    public abstract class ClientLookupTController<T> : ReadController<T> where T : class, IEntity, new()
    {
        [HttpGet]
        public List<T> All()
        {
            return TBusiness.GetAll();
        }
    }
}
