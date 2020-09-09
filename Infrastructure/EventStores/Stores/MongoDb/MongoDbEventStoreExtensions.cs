using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.EventStores.Stores.MongoDb
{
    public static class MongoDbEventStoreExtensions
    {
        public static IServiceCollection AddMongoDbEventStore(this IServiceCollection services, IConfiguration Configuration)
        {
            var options = new MongoDbEventStoreOptions();
            Configuration.GetSection(nameof(EventStoresOptions)).Bind(options);
            services.Configure<MongoDbEventStoreOptions>(Configuration.GetSection(nameof(EventStoresOptions)));

            services.AddScoped<IStore, MongoDbEventStore>();

            return services;
        }
    }
}
