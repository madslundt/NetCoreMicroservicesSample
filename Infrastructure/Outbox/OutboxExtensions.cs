using Infrastructure.Outbox.Stores.MongoDb;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Infrastructure.Outbox
{
    public static class OutboxExtensions
    {
        public static IServiceCollection AddOutbox(this IServiceCollection services, IConfiguration Configuration)
        {
            var options = new OutboxOptions();
            Configuration.GetSection(nameof(OutboxOptions)).Bind(options);
            services.Configure<OutboxOptions>(Configuration.GetSection(nameof(OutboxOptions)));

            switch (options.OutboxType.ToLowerInvariant())
            {
                case "mongo":
                case "mongodb":
                    services.AddMongoDbOutbox(Configuration);
                    break;
                default:
                    throw new Exception($"Outbox type '{options.OutboxType}' is not valid");
            }

            services.AddSingleton<IOutboxListener, OutboxListener>();
            services.AddHostedService<OutboxProcessor>();

            return services;
        }
    }
}
