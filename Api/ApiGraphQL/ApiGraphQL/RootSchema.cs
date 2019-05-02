using ApiGraphQL.Mutations;
using ApiGraphQL.Queries;
using GraphQL;
using GraphQL.Types;

namespace ApiGraphQL
{
    public class RootSchema : Schema
    {
        public RootSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<RootQuery>();
            Mutation = resolver.Resolve<RootMutation>();
        }
    }
}
