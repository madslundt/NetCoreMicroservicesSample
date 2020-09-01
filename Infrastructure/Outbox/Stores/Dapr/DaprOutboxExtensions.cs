using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Infrastructure.Outbox.Stores.Dapr
{
    public static class DaprOutboxExtensions
    {
        public static IServiceCollection AddDaprOutbox(this IServiceCollection services, IConfiguration Configuration)
        {
            throw new NotImplementedException("Dapr is not ready yet");

            var options = new DaprOutboxOptions();
            Configuration.GetSection(nameof(OutboxOptions)).Bind(options);
            services.Configure<DaprOutboxOptions>(Configuration.GetSection(nameof(OutboxOptions)));

            return services;
        }
    }
}
