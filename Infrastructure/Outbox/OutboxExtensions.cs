using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Infrastructure.Outbox
{
    public static class OutboxExtensions
    {
        public static IServiceCollection AddOutbox(this IServiceCollection services, IConfiguration Configuration)
        {
            var options = new OutboxOptions();
            Configuration.GetSection(nameof(OutboxOptions)).Bind(options);
            services.Configure<OutboxOptions>(Configuration.GetSection(nameof(OutboxOptions)));

            services.AddSingleton<IOutboxListener, OutboxListener>();

            switch (options.EventBusType)
            {
                case EventBusTypeEnum.RabbitMQ:
                    services.AddHostedService<OutboxProcessorRabbitMQ>();
                    break;
                case EventBusTypeEnum.Kafka:
                    services.AddHostedService<OutboxProcessorKafka>();
                    break;
                default:
                    throw new NotImplementedException(nameof(EventBusTypeEnum));
            }

            return services;
        }
    }
}
