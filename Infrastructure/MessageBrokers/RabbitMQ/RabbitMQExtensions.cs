using Infrastructure.Core.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RawRabbit.DependencyInjection.ServiceCollection;
using RawRabbit.Instantiation;
using System;

namespace Infrastructure.MessageBrokers.RabbitMQ
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
        public static IApplicationBuilder UseRabbitMQSubscribe<T>(this IApplicationBuilder app) where T : IEvent
        {
            app.ApplicationServices.GetRequiredService<IEventListener>().Subscribe<T>();

            return app;
        }

        public static IApplicationBuilder UseRabbitMQSubscribe(this IApplicationBuilder app, Type type)
        {
            app.ApplicationServices.GetRequiredService<IEventListener>().Subscribe(type);

            return app;
        }
    }
}
