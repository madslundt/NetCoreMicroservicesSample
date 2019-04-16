using Microsoft.AspNetCore.Http;
using System;

namespace ApiGateway.Infrastructure.GraphQL
{
    public class GraphQLSettings
    {
        public Func<HttpContext, object> BuildUserContext { get; set; }

    }
}
