using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RawRabbit;
using System;
using System.Collections.Generic;

namespace MicroserviceBase.RabbitMQ
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

        public void ListenTo(List<Type> eventsToSubscribe)
        {
            foreach (var evtType in eventsToSubscribe)
            {
                //add check if is INotification
                GetType()
                    .GetMethod("Subscribe", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .MakeGenericMethod(evtType)
                    .Invoke(this, new object[] { });
            }
        }

        private void Subscribe<T>() where T : INotification
        {
            //TODO: move exchange name and queue prefix to cfg
            _busClient.SubscribeAsync<T>(
                async (msg) =>
                {
                    //add logging
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var internalBus = scope.ServiceProvider.GetRequiredService<IMediator>();
                        await internalBus.Publish(msg);
                    }
                },
                cfg => cfg.UseSubscribeConfiguration(
                    c => c
                    .OnDeclaredExchange(e => e
                        .WithName("EXCHANGE_NAME")
                        .WithType(RawRabbit.Configuration.Exchange.ExchangeType.Topic)
                        .WithArgument("key", typeof(T).Name.ToLower()))
                    .FromDeclaredQueue(q => q.WithName("QUEUE_NAME" + typeof(T).Name)))
                );
        }
    }
}
