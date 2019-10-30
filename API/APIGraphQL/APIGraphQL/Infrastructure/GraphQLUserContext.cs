using System.Security.Claims;

namespace APIGraphQL.Infrastructure
{
    public class GraphQLUserContext
    {
        public ClaimsPrincipal User { get; set; }
    }
}
