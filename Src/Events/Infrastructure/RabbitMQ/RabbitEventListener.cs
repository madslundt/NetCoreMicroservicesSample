using Events.Infrastructure.Event;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RawRabbit;
using System;
using System.Threading.Tasks;

namespace Events.Infrastructure.RabbitMQ
{
    public interface IRabbitEventListener
    {
        void SubscribeAsync<T>() where T : IEvent;
        Task DispatchAsync<T>(T @event) where T : IEvent;

    }
    public class RabbitEventListener : IRabbitEventListener
    {
        private readonly IBusClient _busClient;
        private readonly IServiceScopeFactory _serviceFactory;
        private readonly RabbitOptions _rabbitOptions;

        public RabbitEventListener(
            IBusClient busClient,
            IOptions<RabbitOptions> options,
            IServiceScopeFactory serviceFactory)
        {
            _busClient = busClient;
            _serviceFactory = serviceFactory;
            _rabbitOptions = options.Value;
        }

        public void SubscribeAsync<T>() where T : IEvent
        {
            _busClient.SubscribeAsync(
                (Func < T, Task > )(async (msg) =>
                {
                    using (var scope = _serviceFactory.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetService<IMediator>();
                        await mediator.Publish(msg);
                    }
                }),
                cfg => cfg.UseSubscribeConfiguration(
                    c => c
                    .OnDeclaredExchange(GetExchangeDeclaration<T>())
                    .FromDeclaredQueue(q => q.WithName((_rabbitOptions.Queue.Name ?? AppDomain.CurrentDomain.FriendlyName).Trim().Trim('_') + "_" + typeof(T).Name)))
            );
        }

        private Action<RawRabbit.Configuration.Exchange.IExchangeDeclarationBuilder> GetExchangeDeclaration<T>() where T : IEvent
        {
            return e => e
                .WithName(_rabbitOptions.Exchange.Name)
                .WithArgument("key", typeof(T).Name.ToLower());
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
                    .OnDeclaredExchange(GetExchangeDeclaration<T>())
                )
            );
        }
    }
}
