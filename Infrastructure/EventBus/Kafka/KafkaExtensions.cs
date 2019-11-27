using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.EventBus.Kafka
{
    public static class KafkaExtensions
    {
        public static IServiceCollection AddKafka(this IServiceCollection services, IConfiguration Configuration)
        {
            var options = new KafkaOptions();
            Configuration.GetSection(nameof(KafkaOptions)).Bind(options);
            services.Configure<KafkaOptions>(Configuration.GetSection(nameof(KafkaOptions)));

            services.AddSingleton<IEventListener, KafkaListener>();

            return services;
        }

        public static IApplicationBuilder UseKafkaSubscribeEvent<T>(this IApplicationBuilder app) where T : IEvent
        {
            app.ApplicationServices.GetRequiredService<IEventListener>().SubscribeEvent<T>();

            return app;
        }

        public static IApplicationBuilder UseKafkaSubscribeCommand<T>(this IApplicationBuilder app) where T : ICommand
        {
            app.ApplicationServices.GetRequiredService<IEventListener>().SubscribeCommand<T>();

            return app;
        }
    }
}
