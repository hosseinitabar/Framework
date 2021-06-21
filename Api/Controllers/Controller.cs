using Holism.Business;
using Holism.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Holism.Api.Controllers
{
    public abstract class Controller<Read, Write> : ReadController<Read> where Read : class, IEntity, new() where Write : class, IEntity, new()
    {
        public abstract Business<Read, Write> Business { get; }

        public virtual Action<Write> BeforeCreation { get; }

        public virtual Action<Write> BeforeUpserting { get; }

        public virtual Action<long> BeforeDeleting { get; }

        public virtual Action<List<long>> BeforeDeletingItems { get; }

        [HttpPost]
        public virtual IActionResult Create(Write model)
        {
            BeforeCreation?.Invoke(model);
            var createdEntity = Business.Create(model, GetExtraParameters());
            return OkJson("Done", createdEntity, "CretionDone");
        }

        [HttpPost]
        public virtual IActionResult Upsert(Write model)
        {
            BeforeUpserting?.Invoke(model);
            var upsertedEntity = Business.Upsert(model, GetExtraParameters());
            return OkJson("Done", upsertedEntity, "UpsertDone");
        }

        public virtual object GetExtraParameters()
        {
            return null;
        }

        [HttpPost]
        public virtual IActionResult Delete(long id)
        {
            BeforeDeleting?.Invoke(id);
            Business.Delete(id);
            return OkJson();
        }

        [HttpPost]
        public virtual IActionResult DeleteItems(List<long> ids)
        {
            BeforeDeletingItems?.Invoke(ids);
            Business.Delete(ids);
            return OkJson();
        }
    }
}
