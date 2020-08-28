using Infrastructure.EventStores.Aggregate;
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
        public static IServiceCollection AddEventStore<TAggregate>(this IServiceCollection services, IConfiguration Configuration, Action<DbContextOptionsBuilder> dbContextOptions = null) where TAggregate : IAggregate
        {
            var options = new EventStoresOptions();
            Configuration.GetSection(nameof(EventStoresOptions)).Bind(options);
            services.Configure<EventStoresOptions>(Configuration.GetSection(nameof(EventStoresOptions)));

            switch (options.EventStoreType.ToLowerInvariant())
            {
                case "efcore":
                case "ef":
                    services.AddEfCoreEventStore(dbContextOptions);
                    break;
                case "mongo":
                case "mongodb":
                    services.AddMongoDbEventStore(Configuration);
                    break;
                default:
                    throw new Exception($"Event store type '{options.EventStoreType}' is not supported");
            }

            services.AddScoped<IRepository<TAggregate>, Repository<TAggregate>>();
            services.AddScoped<IEventStore, EventStore>();

            return services;
        }
    }
}
