using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RawRabbit;
using System;
using System.Threading.Tasks;

namespace Infrastructure.EventBus.RabbitMQ
{
    public class RabbitMQListener : IEventListener
    {
        private readonly IBusClient _busClient;
        private readonly IServiceScopeFactory _serviceFactory;
        private readonly RabbitMQOptions _options;

        public RabbitMQListener(
            IBusClient busClient,
            IOptions<RabbitMQOptions> options,
            IServiceScopeFactory serviceFactory)
        {
            _busClient = busClient;
            _serviceFactory = serviceFactory;
            _options = options.Value;
        }

        public void Subscribe<T>() where T : IEvent
        {
            Subscribe(typeof(T));
        }

        public void Subscribe(Type type)
        {
            _busClient.SubscribeAsync(
                (Func<IEvent, Task>)(async (msg) =>
                {
                    using (var scope = _serviceFactory.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetService<IMediator>();
                        await mediator.Publish(msg);
                    }
                }),
                cfg => cfg.UseSubscribeConfiguration(
                    c => c
                    .OnDeclaredExchange(GetExchangeDeclaration(type))
                    .FromDeclaredQueue(q => q.WithName((_options.Queue.Name ?? AppDomain.CurrentDomain.FriendlyName).Trim().Trim('_') + "_" + type.Name)))
            );
        }


        public async Task Publish<T>(T @event) where T : IEvent
        {
            if (@event == null)
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

        public async Task Publish(string message, string type)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message), "Event message can not be null.");
            }

            if (string.IsNullOrWhiteSpace(type))
            {
                throw new ArgumentNullException(nameof(type), "Event type can not be null.");
            }

            await _busClient.PublishAsync(
                message,
                cfg => cfg.UsePublishConfiguration(
                    c => c
                    .OnDeclaredExchange(GetExchangeDeclaration(type))
                )
            );
        }

        private Action<RawRabbit.Configuration.Exchange.IExchangeDeclarationBuilder> GetExchangeDeclaration<T>()
        {
            return GetExchangeDeclaration(typeof(T));
        }

        private Action<RawRabbit.Configuration.Exchange.IExchangeDeclarationBuilder> GetExchangeDeclaration(Type type)
        {
            var name = EventBusHelper.GetTypeName(type);

            return GetExchangeDeclaration(name);
        }

        private Action<RawRabbit.Configuration.Exchange.IExchangeDeclarationBuilder> GetExchangeDeclaration(string name)
        {
            return e => e
                .WithName(_options.Exchange.Name)
                .WithArgument("key", name);
        }
    }
}
