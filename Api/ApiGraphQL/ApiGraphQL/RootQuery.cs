using GraphQL.Types;

namespace ApiGraphQL
{
    public class RootQuery : ObjectGraphType
    {
        public RootQuery()
        {
            Name = "Query";
        }
    }
}
