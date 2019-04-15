using System.Security.Claims;

namespace ApiGraphQL.Infrastructure.GraphQL
{
    public class GraphQLUserContext
    {
        public ClaimsPrincipal User { get; set; }
    }
}
