using Infrastructure.EventBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Events
{
    public static class EventsExtensions
    {
        public static IApplicationBuilder UseSubscribeAllEvents(this IApplicationBuilder app)
        {
            var types = typeof(EventsExtensions).GetTypeInfo().Assembly.GetTypes()
                .Where(mytype => mytype.GetInterfaces().Contains(typeof(IEvent)));

            foreach (var type in types)
            {
                app.ApplicationServices.GetRequiredService<IEventListener>().Subscribe(type);
            }

            return app;
        }
    }
}
