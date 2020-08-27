using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RawRabbit.DependencyInjection.ServiceCollection;
using RawRabbit.Instantiation;

namespace Infrastructure.MessageBrokers.RabbitMQ
{
    public static class RabbitMQExtensions
    {
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration Configuration)
        {
            var options = new RabbitMQOptions();
            Configuration.GetSection(nameof(MessageBrokersOptions)).Bind(options);
            services.Configure<RabbitMQOptions>(Configuration.GetSection(nameof(MessageBrokersOptions)));

            services.AddRawRabbit(new RawRabbitOptions
            {
                ClientConfiguration = options
            });

            services.AddSingleton<IEventListener, RabbitMQListener>();

            return services;
        }
    }
}
