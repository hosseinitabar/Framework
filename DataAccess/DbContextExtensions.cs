﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Holism.DataAccess
{
    public static class DbContextExtensions
    {
        public static string GetSchemaName(this DbContext context)
        {
            //ObjectContext objContext = ((IObjectContextAdapter)context).ObjectContext;
            //var fullNameTypes = objContext.MetadataWorkspace.GetItems<EntityType>(DataSpace.OSpace);
            //var conStr = objContext.Connection.ConnectionString;
            //var connection = new EntityConnection(conStr);
            //var workspace = connection.GetMetadataWorkspace();
            //var entitySets = workspace.GetItems<EntityContainer>(DataSpace.SSpace).First().BaseEntitySets;
            //for (int i = 0; i < fullNameTypes.Count; i++)
            //{
            //    Type type = Type.GetType(fullNameTypes[i].FullName);
            //    string schema = entitySets[type.Name].MetadataProperties["Schema"].Value.ToString();
            //}
            throw new NotImplementedException();
        }

        public static string GetTableName()
        {
            throw new NotImplementedException();
        }

        public static string GetFullyQualifiedName()
        {
            throw new NotImplementedException();
        }
    }
}
