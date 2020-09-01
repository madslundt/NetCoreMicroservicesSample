using Infrastructure.Core.Events;
using Infrastructure.MessageBrokers.Dapr;
using Infrastructure.MessageBrokers.Kafka;
using Infrastructure.MessageBrokers.RabbitMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Infrastructure.MessageBrokers
{
    public static class MessageBrokersExtensions
    {
        public static IServiceCollection AddMessageBroker(this IServiceCollection services, IConfiguration Configuration)
        {
            var options = new MessageBrokersOptions();
            Configuration.GetSection(nameof(MessageBrokersOptions)).Bind(options);
            services.Configure<MessageBrokersOptions>(Configuration.GetSection(nameof(MessageBrokersOptions)));

            switch (options.MessageBrokerType.ToLowerInvariant())
            {
                case "dapr":
                    return services.AddDapr(Configuration);
                case "rabbitmq":
                    return services.AddRabbitMQ(Configuration);
                case "kafka":
                    return services.AddKafka(Configuration);
                default:
                    throw new Exception($"Message broker type '{options.MessageBrokerType}' is not supported");
            }
        }

        public static IApplicationBuilder UseSubscribeEvent<T>(this IApplicationBuilder app) where T : IEvent
        {
            app.ApplicationServices.GetRequiredService<IEventListener>().Subscribe<T>();

            return app;
        }

        public static IApplicationBuilder UseSubscribeEvent(this IApplicationBuilder app, Type type)
        {
            app.ApplicationServices.GetRequiredService<IEventListener>().Subscribe(type);

            return app;
        }
    }
}
