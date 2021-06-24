using Holism.Framework;
using Holism.Models;
using Holism.DataAccess;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Dynamic;

namespace Holism.Business
{
    public abstract class Business<Read, Write> : ReadBusiness<Read> where Read : class, IEntity, new() where Write : class, IEntity, new()
    {
        protected abstract Repository<Write> WriteRepository { get; }

        public virtual void Validate(Write model)
        {
        }

        protected virtual void BeforeCreation(Write model, object extraParameters = null)
        {
        }

        public Write Create(Write model, object extraParameters = null)
        {
            Validate(model);
            BeforeCreation(model, extraParameters);
            var result = WriteRepository.Create(model);
            PostCreation(model);
            return result;
        }

        protected virtual void PostCreation(Write model)
        {

        }

        protected virtual void BeforeUpdate(Write model, object extraParameters = null)
        {
        }

        public Write Update(Write model, object extraParameters = null)
        {
            return Update(null, model, extraParameters);
        }

        public Write Update(string databaseName, Write model, object extraParameters = null)
        {
            Validate(model);
            BeforeUpdate(model, extraParameters);
            var result = WriteRepository.Update(databaseName, model);
            PostUpdate(model);
            return result;
        }

        protected virtual void PostUpdate(Write model)
        {

        }

        public Write Upsert(Write model, object extraParameters = null)
        {
            Validate(model);
            var isCreation = !WriteRepository.Exists(model);
            if (isCreation)
            {
                BeforeCreation(model, extraParameters);
            }
            else
            {
                BeforeUpdate(model, extraParameters);
            }
            var result = WriteRepository.Upsert(model);
            var relatedItems = (ExpandoObject)result.GetType().GetProperty("RelatedItems").GetValue(model);
            if (ExpandoObjectExtensions.Has(relatedItems, "IsCreation"))
            {
                PostCreation(model);
            }
            else
            {
                PostUpdate(model);
            }
            return result;
        }

        protected virtual void BeforeDeletion(Write model)
        {

        }

        public virtual void Delete(Write model)
        {
            BeforeDeletion(model);
            WriteRepository.Delete(model);
            PostDeletion(model);
        }

        public virtual void Delete(long id)
        {
            var view = Get(id);
            Delete(view.CastTo<Write>());
        }

        public virtual void Delete(List<long> ids)
        {
            if (ids.Count == 0)
            {
                return;
            }
            var items = GetList(ids);
            foreach (var item in items)
            {
                BeforeDeletion(item.CastTo<Write>());
            }
            WriteRepository.Run($@"
                delete
                from {WriteRepository.TableName}
                where Id in 
                (
                    {ids.ToCsv()}
                )
            ");
            foreach (var item in items)
            {
                PostDeletion(item.CastTo<Write>());
            }
        }

        protected virtual void PostDeletion(Write model)
        {

        }

        private List<Read> KeepOriginalOrder(List<Read> items, List<long> orderedIds)
        {
            throw new NotImplementedException();
            //var orderdItems = items.OrderBy(i => orderedIds.IndexOf(i.Id)).ToList();
            //return orderdItems;
        }
    }
}