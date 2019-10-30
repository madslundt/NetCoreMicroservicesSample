using Newtonsoft.Json.Linq;

namespace APIGraphQL.Infrastructure
{
    public class GraphQLRequest
    {
        public class GraphQLRequest
        {
            public string OperationName { get; set; }
            public string Query { get; set; }
            public JObject Variables { get; set; }
        }
    }
}
