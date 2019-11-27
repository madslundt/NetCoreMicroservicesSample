using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RawRabbit.DependencyInjection.ServiceCollection;
using RawRabbit.Instantiation;

namespace Infrastructure.EventBus.RabbitMQ
{
    public static class RabbitMQExtensions
    {
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration Configuration)
        {
            var options = new RabbitMQOptions();
            Configuration.GetSection(nameof(RabbitMQOptions)).Bind(options);
            services.Configure<RabbitMQOptions>(Configuration.GetSection(nameof(RabbitMQOptions)));

            services.AddRawRabbit(new RawRabbitOptions
            {
                ClientConfiguration = options
            });

            services.AddSingleton<IEventListener, RabbitMQListener>();

            return services;
        }
        public static IApplicationBuilder UseRabbitMQSubscribeEvent<T>(this IApplicationBuilder app) where T : IEvent
        {
            app.ApplicationServices.GetRequiredService<IEventListener>().SubscribeEvent<T>();

            return app;
        }

        public static IApplicationBuilder UseRabbitMQSubscribeCommand<T>(this IApplicationBuilder app) where T : ICommand
        {
            app.ApplicationServices.GetRequiredService<IEventListener>().SubscribeCommand<T>();

            return app;
        }
    }
}
