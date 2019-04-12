using App.Metrics;
using App.Metrics.Timer;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Base.Metrics
{
    public class MetricsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IMetricsRoot _metrics;

        public MetricsBehavior(IMetricsRoot metrics)
        {
            _metrics = metrics;
        }
        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var requestTimer = new TimerOptions
            {
                Name = "Mediator Pipeline",
                MeasurementUnit = App.Metrics.Unit.Requests,
                DurationUnit = TimeUnit.Milliseconds,
                RateUnit = TimeUnit.Milliseconds
            };

            TResponse response;
            using (_metrics.Measure.Timer.Time(requestTimer))
            {
                response = await next();
            }

            _metrics.ReportRunner.RunAllAsync();

            return response;
        }
    }
}
