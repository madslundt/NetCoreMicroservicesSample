using Infrastructure.Core.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.MessageBrokers.Kafka
{
    public static class KafkaExtensions
    {
        public static IServiceCollection AddKafka(this IServiceCollection services, IConfiguration Configuration)
        {
            var options = new KafkaOptions();
            Configuration.GetSection(nameof(MessageBrokersOptions)).Bind(options);
            services.Configure<KafkaOptions>(Configuration.GetSection(nameof(MessageBrokersOptions)));

            services.AddSingleton<IEventListener, KafkaListener>();

            return services;
        }

        public static IApplicationBuilder UseKafkaSubscribe<T>(this IApplicationBuilder app) where T : IEvent
        {
            app.ApplicationServices.GetRequiredService<IEventListener>().Subscribe<T>();

            return app;
        }
    }
}
