using Events.Infrastructure.Event;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Events.Infrastructure.RabbitMQ
{
    public static class RabbitExtensions
    {
        public static void AddRabbit(this IConfigurationRoot configuration)
        {
            var rabbitOptions = new RabbitOptions();
            configuration.GetSection(nameof(RabbitOptions)).Bind(rabbitOptions);
        }
        public static void UseRabbitSubscribe<T>(this IApplicationBuilder app) where T : IEvent
        {
            app.ApplicationServices.GetRequiredService<RabbitEventListener>().SubscribeAsync<T>();
        }
    }
}
