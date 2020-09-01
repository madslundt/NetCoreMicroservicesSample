using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Infrastructure.MessageBrokers.Dapr
{
    public static class DaprExtensions
    {
        public static IServiceCollection AddDapr(this IServiceCollection services, IConfiguration Configuration)
        {
            throw new NotImplementedException("Dapr is not ready yet");

            var options = new DaprOptions();
            Configuration.GetSection(nameof(MessageBrokersOptions)).Bind(options);
            services.Configure<DaprOptions>(Configuration.GetSection(nameof(MessageBrokersOptions)));

            services
                .AddControllers()
                .AddDapr();

            return services;
        }

        public static IApplicationBuilder UseDapr(this IApplicationBuilder app)
        {
            throw new NotImplementedException("Dapr is not ready yet");

            app.UseCloudEvents();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapSubscribeHandler();
                endpoints.MapControllers();
            });

            return app;
        }
    }
}
