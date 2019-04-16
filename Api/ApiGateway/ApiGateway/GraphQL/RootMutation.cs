using GraphQL.Types;

namespace ApiGateway.GraphQL
{
    public class RootMutation : ObjectGraphType
    {
        public RootMutation()
        {
            Name = "Mutation";
        }
    }
}
