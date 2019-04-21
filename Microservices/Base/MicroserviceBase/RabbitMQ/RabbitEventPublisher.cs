using MicroserviceBase.Messaging;
using RawRabbit;
using System.Threading.Tasks;

namespace MicroserviceBase.RabbitMQ
{
    public class RabbitEventPublisher : IEventPublisher
    {
        private readonly IBusClient busClient;

        public RabbitEventPublisher(IBusClient busClient)
        {
            this.busClient = busClient;
        }

        public Task PublishMessage<T>(T msg)
        {
            return busClient.BasicPublishAsync(msg, cfg => {
                cfg.OnExchange("EXCHANGE_NAME").WithRoutingKey(typeof(T).Name.ToLower());
            });
        }
    }
}
