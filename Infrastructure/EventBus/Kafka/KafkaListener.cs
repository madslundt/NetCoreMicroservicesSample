using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Infrastructure.EventBus.Kafka
{
    public class KafkaListener : IEventListener
    {
        private KafkaOptions _kafkaOptions;
        private readonly ProducerConfig _producerConfig;
        private ConsumerConfig _consumerConfig;

        public KafkaListener(IOptions<KafkaOptions> options)
        {
            _kafkaOptions = options.Value;
            // TODO
            //var consumerConfig = _kafkaOptions.ConsumerOptions.GetType()
            // .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            //      .ToDictionary(prop => prop.Name, prop => prop.GetValue(_kafkaOptions.ConsumerOptions, null));


            //_producerConfig = new ProducerConfig(_kafkaOptions);
            _producerConfig = new ProducerConfig { BootstrapServers = "localhost:9092" };
            //_consumerConfig = new ConsumerConfig(_kafkaOptions);
            _consumerConfig = new ConsumerConfig
            {
                GroupId = "testms",
                BootstrapServers = "localhost:9092",
                // Note: The AutoOffsetReset property determines the start offset in the event
                // there are not yet any committed offsets for the consumer group for the
                // topic/partitions of interest. By default, offsets are committed
                // automatically, so in this example, consumption will only start from the
                // earliest message in the topic 'my-topic' the first time you run the program.
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
        }

        public void Subscribe<T>() where T : IEvent
        {
            Subscribe(typeof(T));
        }

        public void Subscribe(Type type)
        {
            using (var c = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build())
            {
                var name = EventBusHelper.GetTypeName(type);
                c.Subscribe(name);

                try
                {
                    while (true)
                    {
                        c.Consume();
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    c.Close();
                    throw;
                }
            }
        }

        public async Task Publish<T>(T @event) where T : IEvent
        {
            using (var p = new ProducerBuilder<Null, T>(_producerConfig).Build())
            {
                var name = EventBusHelper.GetTypeName<T>();
                var dr = await p.ProduceAsync(name, new Message<Null, T> { Value = @event });
            }
        }

        public async Task Publish(string message, string type)
        {
            using (var p = new ProducerBuilder<Null, string>(_producerConfig).Build())
            {
                var dr = await p.ProduceAsync(type, new Message<Null, string> { Value = message });
            }
        }
    }
}
