using Infrastructure.EventStores.Aggregate;
using Infrastructure.EventStores.Stores.MongoDb;
using Infrastructure.Outbox.Providers.MongoDb;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Outbox.Stores.MongoDb
{
    public static class MongoDbOutboxExtensions
    {
        public static IServiceCollection AddMongoDbOutbox(this IServiceCollection services, IConfiguration Configuration)
        {
            var options = new MongoDbOutboxOptions();
            Configuration.GetSection(nameof(OutboxOptions)).Bind(options);
            services.Configure<MongoDbOutboxOptions>(Configuration.GetSection(nameof(OutboxOptions)));

            services.AddScoped<IOutboxStore, MongoDbOutboxStore>();

            return services;
        }
    }
}
