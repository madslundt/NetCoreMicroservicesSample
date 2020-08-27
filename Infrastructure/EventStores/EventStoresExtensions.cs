using Infrastructure.EventStores.Aggregates;
using Infrastructure.EventStores.Repository;
using Infrastructure.EventStores.Stores.EfCore;
using Infrastructure.EventStores.Stores.MongoDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Infrastructure.EventStores
{
    public static class EventStoresExtensions
    {
        public static IServiceCollection AddEventStore<TAggregate>(this IServiceCollection services, IConfiguration Configuration = null, Action<DbContextOptionsBuilder> dbContextOptions = null) where TAggregate : IAggregate
        {
            var options = new EventStoresOptions();
            Configuration.GetSection(nameof(EventStoresOptions)).Bind(options);
            services.Configure<EventStoresOptions>(Configuration.GetSection(nameof(EventStoresOptions)));

            switch (options.EventStoreType.ToLowerInvariant())
            {
                case "efcore":
                case "ef":
                    services.AddEfCoreDbEventStore(dbContextOptions);
                    break;
                case "mongo":
                case "mongodb":
                    services.AddMongoDbEventStore(Configuration);
                    break;
                default:
                    throw new Exception($"Event store type '{options.EventStoreType}' is not valid");
            }

            services.AddScoped<IRepository<TAggregate>, Repository<TAggregate>>();
            services.AddScoped<IEventStore, EventStore>();

            return services;
        }
    }
}
