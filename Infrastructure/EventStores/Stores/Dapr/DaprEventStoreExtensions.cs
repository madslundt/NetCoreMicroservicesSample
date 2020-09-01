using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EventStores.Stores.Dapr
{
    public static class DaprEventStoreExtensions
    {
        public static IServiceCollection AddDaprEventStore(this IServiceCollection services, IConfiguration Configuration)
        {
            throw new NotImplementedException("Dapr is not ready yet");

            var options = new DaprEventStoreOptions();
            Configuration.GetSection(nameof(EventStoresOptions)).Bind(options);
            services.Configure<DaprEventStoreOptions>(Configuration.GetSection(nameof(EventStoresOptions)));

            return services;
        }
    }
}
