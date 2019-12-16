# Sitecore JSS GraphQL Extensions
An unofficial collection of GraphQL extensions for Sitecore JSS GraphQL endpoints. To learn more about extending and querying GraphQL, visit the official documentation [here](https://jss.sitecore.com/docs/techniques/graphql/graphql-overview).

## Who Am I Query
Ever want to query the current Sitecore user profile data with GraphQL? This example was pulled from the official Sitecore JSS documentation but it seemed to need a home. To get started using this query, add the following to your Sitecore JSS application's configuration file in the `list:AddSchemaProvider` of your GraphQL endpoint:
```
<whoAmI type="GraphQLExtensions.WhoAmI.WhoAmISchemaProvider, GraphQLExtensions" />
```
After this is in place, you should see `whoAmI` on your `query` GraphQL type in the Documentation Explorer and you should be able to dispatch queries that look like the following:
```
{
  whoAmI {
    name
  }
}
```
## Ancestors
Implementing breadcrumbs but can't traverse up the tree from the current page? This extender enables querying for a single ancestor with some conditions or enumerating all ancestors of a particular item.  To get started using this query, add the following to your Sitecore JSS application's configuration file in the `list:AddExtender` of your GraphQL endpoint:
```
<ancestorExtender type="GraphQLExtensions.Ancestor.AncestorExtender, GraphQLExtensions" />
```
After this in place, you should see `ancestor` and `ancestors` fields on the `Item`  GraphQL type in the Documentation Explorer and you should be able to dispatch queries that look like the following:

**Ancestor Example:**
```
{
  item(path:"/sitecore/content/reacttest/home/graphql") {
    ancestor(name:"home") {
      name
      url
    }
  }
}
```
The following fields are available as parameters for `ancestor`:

 - `requirePresentation`: Only return items that have presentation details assigned to them.
 - `includeSelf`: Also consider the current item for any potential matches.
 - `includeTemplateIDs` Filter ancestors by an array of template ID's.
 - `name` The name of the item to search for.

**Ancestors Example:**
```
{
  item(path:"/sitecore/content/reacttest/home/graphql") {
    ancestors(requirePresentation:true, includeSelf:true) {
      name
      url
    }
  }
}
```
The following fields are available as parameters for `ancestors`:

 - `requirePresentation`: Only return items that have presentation details assigned to them.
 - `includeSelf`: Also consider the current item for any potential matches.
 - `includeTemplateIDs` Filter ancestors by an array of template ID's.

## Contributing
Have a query or extension you think might be a good fit here? It probably is! All pull requests are welcome.