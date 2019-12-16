using GraphQL.Types;
using Sitecore.Services.GraphQL.Content.GraphTypes;
using Sitecore.Services.GraphQL.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Query;
using Sitecore.Pipelines.HasPresentation;

namespace GraphQLExtensions.Ancestor
{
    public class AncestorExtender : SchemaExtender
    {
        public AncestorExtender()
        {
            QueryArguments ancestorArguments = BuildAncestorArguments();

            ExtendTypes<ItemGraphType>(type =>
            {
                type.Field<ItemInterfaceGraphType>("ancestor", null, ancestorArguments, ResolveAncestor);
            });

            ExtendTypes<ItemInterfaceGraphType>(type =>
            {
                type.Field<ItemInterfaceGraphType>("ancestor", null, ancestorArguments, null);
            });

            QueryArguments ancestorsArguments = BuildAncestorsArguments();

            ExtendTypes<ItemGraphType>(type =>
            {
                type.Field<ListGraphType<ItemInterfaceGraphType>>("ancestors", null, ancestorsArguments, ResolveAncestors);
            });

            ExtendTypes<ItemInterfaceGraphType>(type =>
            {
                type.Field<ListGraphType<ItemInterfaceGraphType>>("ancestors", null, ancestorsArguments, null);
            });
        }

        private QueryArgument[] BuildCommonArguments()
        {
            QueryArgument<BooleanGraphType> presentationArgument = new QueryArgument<BooleanGraphType>
            {
                Name = "requirePresentation",
                Description = "If set only items with existing presentation are returned",
                DefaultValue = false
            };

            QueryArgument<BooleanGraphType> includeSelfArgument = new QueryArgument<BooleanGraphType>
            {
                Name = "includeSelf",
                Description = "Consider the current item in the search",
                DefaultValue = false
            };

            QueryArgument<ListGraphType<StringGraphType>> templateArgument =
                new QueryArgument<ListGraphType<StringGraphType>>
                {
                    Name = "includeTemplateIDs",
                    Description = "Only consider child items with these template IDs.",
                    DefaultValue = new List<string>()
                };

            return new QueryArgument[]
            {
                presentationArgument, 
                includeSelfArgument, 
                templateArgument
            };
        }

        private QueryArguments BuildAncestorArguments()
        {
            QueryArgument[] common = BuildCommonArguments();

            QueryArguments arguments = new QueryArguments(common);

            arguments.Add(new QueryArgument<StringGraphType>
            {
                Name = "name",
                Description = "Name of the item being searched for.",
                DefaultValue = null
            });

            return arguments;
        }

        private QueryArguments BuildAncestorsArguments()
        {
            QueryArgument[] common = BuildCommonArguments();

            QueryArguments arguments = new QueryArguments(common);

            return arguments;
        }

        private IEnumerable<Item> GetAncestors(ResolveFieldContext<Item> context)
        {
            bool requirePresentation = context.GetArgument<bool>("requirePresentation", false);
            bool includeSelf = context.GetArgument<bool>("includeSelf", false);
            ID[] includeTemplates = this.ResolveIdList(context, "includeTemplateIDs").ToArray<ID>();
            
            List<Item> itemsToSearch = new List<Item>(context.Source.Axes.GetAncestors());

            if (includeSelf)
            {
                itemsToSearch.Add(context.Source);
            }

            IEnumerable<Item> source = !requirePresentation ? itemsToSearch : itemsToSearch.Where(HasPresentationPipeline.Run);

            if (includeTemplates.Any())
            {
                source = source.Where(item => includeTemplates.Contains(item.TemplateID));
            }

            return source.Reverse();
        }

        private object ResolveAncestor(ResolveFieldContext<Item> context)
        {
            string name = context.GetArgument<string>("name", null);

            IEnumerable<Item> source = GetAncestors(context);

            Item foundAncestor = source.FirstOrDefault(item => item.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            return foundAncestor;
        }

        private object ResolveAncestors(ResolveFieldContext<Item> context)
        {
            Item[] source = GetAncestors(context).ToArray();
            return source;
        }

        private IEnumerable<ID> ResolveIdList(
            ResolveFieldContext<Item> context,
            string argumentName)
        {
            List<string> parameterValue = context.GetArgument<List<string>>(argumentName, null);
            if (parameterValue != null)
            {
                foreach (string inputPathOrIdOrShortId in parameterValue)
                {
                    ID result;
                    if (IdHelper.TryResolveId(inputPathOrIdOrShortId, out result))
                    {
                        yield return result;
                    }
                    else
                    {
                        context.Errors.Add(new ExecutionError(argumentName + " value " + inputPathOrIdOrShortId +
                                                              " was not a valid ID or short ID!"));
                    }
                }
            }
        }
    }
}
