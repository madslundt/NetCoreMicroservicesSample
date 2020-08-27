using Infrastructure.EventStores.Aggregates;
using Infrastructure.EventStores.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Infrastructure.EventStores.Stores.EfCore
{
    public static class EfCoreEventStoreExtensions
    {
        public static IServiceCollection AddEfCoreEventStore(this IServiceCollection services, Action<DbContextOptionsBuilder> dbContextOptions)
        {
            services.AddDbContext<EfCoreEventStoreContext>(dbContextOptions);
            services.AddSingleton<IStore, EfCoreEventStore>();

            return services;
        }
    }
}
