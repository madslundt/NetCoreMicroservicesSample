using Infrastructure.EventStores.Aggregates;
using Infrastructure.EventStores.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Infrastructure.EventStores.Providers.EfCore
{
    public static class EfCoreEventStoreExtensions
    {
        public static IServiceCollection AddEfCoreDbEventStore<TAggregate>(this IServiceCollection services, Action<DbContextOptionsBuilder> dbContextOptions) where TAggregate : IAggregate
        {
            services.AddDbContext<EventStoreContext>(dbContextOptions);            

            services.AddSingleton<IEventStore, EfCoreEventStore>();
            services.AddScoped<IRepository<TAggregate>, Repository<TAggregate>>();

            return services;
        }
    }
}
