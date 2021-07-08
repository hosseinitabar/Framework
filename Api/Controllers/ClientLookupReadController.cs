using Holism.DataAccess;
using Holism.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Holism.Api.Controllers
{
    public abstract class ClientLookupReadController<T> : ReadController<T> where T : class, IEntity, new()
    {
        [HttpGet]
        public List<T> All()
        {
            return ReadBusiness.GetAll();
        }
    }
}
