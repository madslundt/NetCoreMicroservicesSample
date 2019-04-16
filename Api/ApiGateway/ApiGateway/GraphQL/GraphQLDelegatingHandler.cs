using ApiGateway.Infrastructure.GraphQL;
using GraphQL;
using GraphQL.Http;
using GraphQL.Instrumentation;
using GraphQL.Server.Transports.AspNetCore.Common;
using GraphQL.Types;
using GraphQL.Validation;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ApiGateway.GraphQL
{
    public class GraphQLDelegatingHandler : DelegatingHandler
    {
        private readonly GraphQLSettings _settings;
        private readonly IDocumentExecuter _executer;
        private readonly IDocumentWriter _writer;
        private readonly ISchema _schema;

        public GraphQLDelegatingHandler(
            GraphQLSettings settings,
            IDocumentExecuter executer,
            IDocumentWriter writer,
            ISchema schema)
        {
            _settings = settings;
            _executer = executer;
            _writer = writer;
            _schema = schema;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var start = DateTime.UtcNow;
            var stream = await request.Content.ReadAsStreamAsync();
            var graphQLRequest = Deserialize<GraphQLRequest>(stream);

            if (graphQLRequest is null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            var result = await _executer.ExecuteAsync(_ =>
            {
                _.Schema = _schema;
                _.Query = graphQLRequest?.Query;
                _.OperationName = graphQLRequest?.OperationName;
                _.Inputs = graphQLRequest?.Variables.ToInputs();
                _.EnableMetrics = true;
                //_.UserContext = _settings.BuildUserContext?.Invoke();
                _.ValidationRules = DocumentValidator.CoreRules().Concat(new[] { new InputValidationRule() });
            });

            var res = await CreateResponseAsync(result);

            result.EnrichWithApolloTracing(start);

            return res;
        }

        private async Task<HttpResponseMessage> CreateResponseAsync(ExecutionResult result)
        {
            var json = await _writer.WriteToStringAsync(result);

            var message = new HttpResponseMessage((result.Errors?.Any() ?? true) ? HttpStatusCode.BadRequest : HttpStatusCode.OK)
            {
                Content = new StringContent(json),
            };

            return message;
        }

        public static T Deserialize<T>(Stream s)
        {
            using (var reader = new StreamReader(s))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var ser = new JsonSerializer();
                return ser.Deserialize<T>(jsonReader);
            }
        }
    }
}
