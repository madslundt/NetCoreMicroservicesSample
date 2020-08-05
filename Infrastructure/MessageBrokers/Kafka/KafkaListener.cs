using Confluent.Kafka;
using Infrastructure.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Infrastructure.MessageBrokers.Kafka
{
    public class KafkaListener : IEventListener
    {
        private KafkaOptions _kafkaOptions;
        private readonly string[] _topics;
        private readonly IServiceScopeFactory _serviceFactory;

        public KafkaListener(IServiceScopeFactory serviceFactory, IOptions<KafkaOptions> options)
        {
            _kafkaOptions = options.Value;
            _topics = _kafkaOptions.Topics.Split(";");
            _serviceFactory = serviceFactory;
        }

        public virtual void Subscribe<T>() where T : IEvent
        {
            Subscribe(typeof(T));
        }

        public virtual void Subscribe(Type type)
        {
            using (var consumer = new ConsumerBuilder<string, string>(_kafkaOptions.Consumer).Build())
            {
                consumer.Subscribe(_topics);
                while (true)
                {
                    var message = consumer.Consume();

                    var @event = JsonConvert.DeserializeObject(message.Message.Value, type) as IEvent;

                    using (var scope = _serviceFactory.CreateScope())
                    {
                        var eventBus = scope.ServiceProvider.GetService<IEventBus>();
                        eventBus.PublishLocal(@event);
                    }
                }
            }
        }

        public virtual async Task Publish<T>(T @event) where T : IEvent
        {
            using (var p = new ProducerBuilder<string, string>(_kafkaOptions.Producer).Build())
            {
                await p.ProduceAsync(_kafkaOptions.Topic,
                    new Message<string, string>
                    {
                        Key = MessageBrokersHelper.GetTypeName<T>(),
                        Value = JsonConvert.SerializeObject(@event)
                    });
            }
        }

        public virtual async Task Publish(string message, string type)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message), "Event message can not be null.");
            }

            if (string.IsNullOrWhiteSpace(type))
            {
                throw new ArgumentNullException(nameof(type), "Event type can not be null.");
            }

            using (var p = new ProducerBuilder<string, string>(_kafkaOptions.Producer).Build())
            {
                await p.ProduceAsync(_kafkaOptions.Topic,
                    new Message<string, string>
                    {
                        Key = type,
                        Value = message
                    });
            }
        }
    }
}
