using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Base.Logging
{
    public class LoggingPreProcessor<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly ILogger<LoggingPreProcessor<TRequest>> _logger;

        public LoggingPreProcessor(ILogger<LoggingPreProcessor<TRequest>> logger)
        {
            _logger = logger;
        }

        public async Task Process(TRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling {typeof(TRequest).FullName}", new
            {
                Request = request
            });
        }
    }
}
