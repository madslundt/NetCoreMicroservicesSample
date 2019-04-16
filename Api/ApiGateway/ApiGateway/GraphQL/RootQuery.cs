using GraphQL.Types;

namespace ApiGateway.GraphQL
{
    public class RootQuery : ObjectGraphType
    {
        public RootQuery()
        {
            Name = "Query";
        }
    }
}
