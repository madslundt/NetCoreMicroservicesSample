using RawRabbit.Configuration;

namespace Events.Infrastructure.RabbitMQ
{
    public class RabbitOptions : RawRabbitConfiguration
    {
        public QueueOptions Queue { get; set; }
        public ExchangeOptions Exchange { get; set; }

        public class QueueOptions : GeneralQueueConfiguration
        {
            public string Name { get; set; }
        }

        public class ExchangeOptions : GeneralExchangeConfiguration
        {
            public string Name { get; set; }
        }
    }
}
