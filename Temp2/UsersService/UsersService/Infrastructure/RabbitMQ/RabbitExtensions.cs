using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using UsersService.Infrastructure.Event;

namespace UsersService.Infrastructure.RabbitMQ
{
    public static class RabbitExtensions
    {
        public static void UseRabbitSubscribe<T>(this IApplicationBuilder app) where T : IEvent
        {
            app.ApplicationServices.GetRequiredService<RabbitEventListener>().SubscribeAsync<T>();
        }
    }
}
