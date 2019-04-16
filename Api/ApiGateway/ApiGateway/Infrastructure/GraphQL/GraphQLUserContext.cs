using System.Security.Claims;

namespace ApiGateway.Infrastructure.GraphQL
{
    public class GraphQLUserContext
    {
        public ClaimsPrincipal User { get; set; }

    }
}
