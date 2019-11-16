using Events.Infrastructure.Event;
using MediatR;
using RawRabbit;
using System;
using System.Threading.Tasks;

namespace Events.Infrastructure.RabbitMQ
{
    public class RabbitEventListener
    {
        private readonly IBusClient _busClient;
        private readonly IMediator _mediator;

        public RabbitEventListener(
            IBusClient busClient,
            IMediator mediator)
        {
            _busClient = busClient;
            _mediator = mediator;
        }

        public void SubscribeAsync<T>() where T : IEvent
        {
            _busClient.SubscribeAsync<T>(
                async (msg) =>
                {
                    await _mediator.Publish(msg);
                }
            );
        }

        public async Task DispatchAsync<T>(T @event) where T : IEvent
        {
            if (@event is null)
            {
                throw new ArgumentNullException(nameof(@event), "Event can not be null.");
            }

            await _busClient.PublishAsync(@event);
        }
    }
}
