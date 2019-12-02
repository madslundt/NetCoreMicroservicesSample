using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Outbox
{
    public static class OutboxExtensions
    {
        public static IServiceCollection AddOutbox(this IServiceCollection services, IConfiguration Configuration)
        {
            var options = new OutboxOptions();
            Configuration.GetSection(nameof(OutboxOptions)).Bind(options);
            services.Configure<OutboxOptions>(Configuration.GetSection(nameof(OutboxOptions)));

            services.AddSingleton<IOutboxListener, OutboxListener>();
            services.AddHostedService<OutboxProcessor>();

            return services;
        }
    }
}
