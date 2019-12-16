using GraphQL.Types;
using Sitecore;
using Sitecore.Security.Accounts;
using Sitecore.Services.GraphQL.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphQLExtensions.WhoAmI
{
    /// <summary>
    /// Teaches GraphQL how to resolve the `whoAmI` root field.
    ///
    /// RootFieldType<UserGraphType, User> means this root field maps a `User` domain object into the `UserGraphType` graph type object.
    /// This code was pulled from https://jss.sitecore.com/docs/techniques/graphql/graphql-overview
    /// </summary>
    internal class WhoAmIQuery : RootFieldType<UserGraphType, User>
    {
        public WhoAmIQuery() : base(name: "whoAmI", description: "Gets the current user")
        {
        }

        protected override User Resolve(ResolveFieldContext context)
        {
            // this is the object the resolver maps onto the graph type
            // (see UserGraphType below). This is your own domain object, not GraphQL-specific.
            return Context.User;
        }
    }
}
