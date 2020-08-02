using Infrastructure.Core.Aggregates;
using Infrastructure.EventStores.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.EventStores.MongoDb
{
    public static class MongoDbExtensions
    {
        public static IServiceCollection AddMongoDbEventStore<TAggregate>(this IServiceCollection services, IConfiguration Configuration) where TAggregate : IAggregate
        {
            var options = new MongoDbEventStoreOptions();
            Configuration.GetSection(nameof(MongoDbEventStoreOptions)).Bind(options);
            services.Configure<MongoDbEventStoreOptions>(Configuration.GetSection(nameof(MongoDbEventStoreOptions)));

            services.AddSingleton<IEventStore, MongoDbEventStore>();
            services.AddScoped<IRepository<TAggregate>, Repository<TAggregate>>();

            return services;
        }
    }
}
