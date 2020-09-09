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
            services.AddScoped<IStore, EfCoreEventStore>();

            return services;
        }
    }
}
