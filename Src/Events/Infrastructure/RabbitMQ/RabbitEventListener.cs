using Events.Infrastructure.Event;
using MediatR;
using RawRabbit;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Events.Infrastructure.RabbitMQ
{
    public class RabbitEventListener
    {
        private readonly IBusClient _busClient;
        private readonly IMediator _mediator;
        private readonly RabbitOptions _rabbitOptions;

        public RabbitEventListener(
            IBusClient busClient,
            IMediator mediator,
            RabbitOptions rabbitOptions)
        {
            _busClient = busClient;
            _mediator = mediator;
            _rabbitOptions = rabbitOptions;
        }

        public void SubscribeAsync<T>() where T : IEvent
        {
            _busClient.SubscribeAsync<T>(
                async (msg) =>
                {
                    await _mediator.Publish(msg);
                },
                cfg => cfg.UseSubscribeConfiguration(
                    c => c
                    .OnDeclaredExchange(e => e
                        .WithName(_rabbitOptions.Exchange.Name)
                        .WithType(RawRabbit.Configuration.Exchange.ExchangeType.Topic)
                        .WithArgument("key", typeof(T).Name.ToLower()))
                    .FromDeclaredQueue(q => q.WithName((_rabbitOptions.Queue.Name ?? Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName)) + typeof(T).Name)))
            );
        }

        public async Task DispatchAsync<T>(T @event) where T : IEvent
        {
            if (@event is null)
            {
                throw new ArgumentNullException(nameof(@event), "Event can not be null.");
            }

            await _busClient.PublishAsync(
                @event,
                cfg => cfg.UsePublishConfiguration(
                    c => c
                    .OnDeclaredExchange(e => e
                        .WithName(_rabbitOptions.Exchange.Name)
                        .WithType(RawRabbit.Configuration.Exchange.ExchangeType.Topic)
                        .WithArgument("key", typeof(T).Name.ToLower())
                    )
                )
            );
        }
    }
}
