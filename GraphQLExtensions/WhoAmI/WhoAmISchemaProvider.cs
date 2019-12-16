using GraphQL.Types;
using Sitecore.Services.GraphQL.Schemas;
using System.Collections.Generic;

namespace GraphQLExtensions.WhoAmI
{
    /// <summary>
    /// Sample of making your own schema provider
    /// This sample enables you to query on the current context user
    /// This code was pulled from https://jss.sitecore.com/docs/techniques/graphql/graphql-overview
    /// </summary>
    public class WhoAmISchemaProvider : SchemaProviderBase
    {
        public override IEnumerable<FieldType> CreateRootQueries()
        {
            yield return new WhoAmIQuery();
        }
    }
}
