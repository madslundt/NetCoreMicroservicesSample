using Infrastructure.Outbox.Stores.Dapr;
using Infrastructure.Outbox.Stores.EfCore;
using Infrastructure.Outbox.Stores.MongoDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Infrastructure.Outbox
{
    public static class OutboxExtensions
    {
        public static IServiceCollection AddOutbox(this IServiceCollection services, IConfiguration Configuration, Action<DbContextOptionsBuilder> dbContextOptions = null)
        {
            var options = new OutboxOptions();
            Configuration.GetSection(nameof(OutboxOptions)).Bind(options);
            services.Configure<OutboxOptions>(Configuration.GetSection(nameof(OutboxOptions)));

            switch (options.OutboxType.ToLowerInvariant())
            {
                case "efcore":
                case "ef":
                    services.AddEfCoreOutboxStore(dbContextOptions);
                    break;
                case "dapr":
                    services.AddDaprOutbox(Configuration);
                    break;
                case "mongo":
                case "mongodb":
                    services.AddMongoDbOutbox(Configuration);
                    break;
                default:
                    throw new Exception($"Outbox type '{options.OutboxType}' is not supported");
            }

            services.AddScoped<IOutboxListener, OutboxListener>();
            services.AddHostedService<OutboxProcessor>();

            return services;
        }
    }
}
