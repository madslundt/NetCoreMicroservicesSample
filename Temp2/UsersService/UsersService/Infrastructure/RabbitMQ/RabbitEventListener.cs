using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RawRabbit;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UsersService.Infrastructure.Event;

namespace UsersService.Infrastructure.RabbitMQ
{
    public class RabbitEventListener
    {
        private readonly IBusClient _busClient;
        private readonly IServiceProvider _serviceProvider;

        public RabbitEventListener(
            IBusClient busClient,
            IServiceProvider serviceProvider)
        {
            _busClient = busClient;
            _serviceProvider = serviceProvider;
        }

        public Task SendAsync<TCommand>(TCommand command)
            where TCommand : class, ICommand
            => _busClient.PublishAsync(command);

        public Task PublishAsync<TEvent>(TEvent @event)
            where TEvent : class, IEvent
            => _busClient.PublishAsync(@event);

        public void ListenTo(List<Type> eventsToSubscribe)
        {
            foreach (var evtType in eventsToSubscribe)
            {
                //add check if is INotification
                this.GetType()
                    .GetMethod("Subscribe", BindingFlags.NonPublic | BindingFlags.Instance)
                    .MakeGenericMethod(evtType)
                    .Invoke(this, new object[] { });
            }
        }

        public async Task SubscribeToCommand<TCommand>() where TCommand : ICommand
            => await _busClient.SubscribeAsync<TCommand>(
            async (msg) =>
            {
                //add logging
                using (var scope = _serviceProvider.CreateScope())
                {
                    var internalBus = scope.ServiceProvider.GetRequiredService<IMediator>();
                    await internalBus.Publish(msg);
                }
            });
            //cfg => cfg.WithQueue(q => q.WithName(GetExchangeName<TCommand>())));

        public async Task SubscribeToEventAsync<TEvent>() where TEvent : IEvent
            => await _busClient.SubscribeAsync<TEvent>(
            async (msg) =>
            {
                //add logging
                using (var scope = _serviceProvider.CreateScope())
                {
                    var internalBus = scope.ServiceProvider.GetRequiredService<IMediator>();
                    await internalBus.Publish(msg);
                }
            });
        //cfg => cfg.UseSubscribeConfiguration(
        //        c => c.
        //        .OnDeclaredExchange(e => e
        //            .WithName("lab-dotnet-micro")
        //            .WithType(RawRabbit.Configuration.Exchange.ExchangeType.Topic)
        //            .WithArgument("key", typeof(T).Name.ToLower()))
        //        .FromDeclaredQueue(q => q.WithName("lab-chat-service-" + typeof(T).Name)))
        //cfg => cfg.WithQueue(q => qp.WithName(GetExchangeName<TEvent>())));

        private string GetExchangeName<T>()
        {
            var assemblyName = Assembly.GetCallingAssembly().GetName().Name;
            var eventName = typeof(T).Name;

            return $"{assemblyName}/{eventName}";
        }
    }

    public static class RabbitListenersInstaller
    {
        public static void UseRabbitListeners(this IApplicationBuilder app, List<Type> eventTypes)
        {
            app.ApplicationServices.GetRequiredService<RabbitEventListener>().ListenTo(eventTypes);
        }
    }
}
