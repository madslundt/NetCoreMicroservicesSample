using GraphQL.Types;

namespace ApiGraphQL.Queries
{
    public class RootQuery : ObjectGraphType
    {
        public RootQuery()
        {
            Name = "Query";
        }
    }
}
