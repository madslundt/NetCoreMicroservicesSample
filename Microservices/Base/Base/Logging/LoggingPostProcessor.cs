using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Base.Logging
{
    public class LoggingPostProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    {
        private readonly ILogger<LoggingPostProcessor<TRequest, TResponse>> _logger;

        public LoggingPostProcessor(ILogger<LoggingPostProcessor<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task Process(TRequest request, TResponse response)
        {
            _logger.LogInformation($"Handled {typeof(TRequest).FullName}", new
            {
                Request = request,
                Response = response
            });
        }
    }
}
