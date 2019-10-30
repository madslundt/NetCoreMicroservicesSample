using Microsoft.AspNetCore.Http;
using System;

namespace APIGraphQL.Infrastructure
{
    public class GraphQLSettings
    {
        public PathString Path { get; set; } = "/api/graphql";
        public Func<HttpContext, object> BuildUserContext { get; set; }
    }
}
