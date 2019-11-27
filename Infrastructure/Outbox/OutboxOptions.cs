namespace Infrastructure.Outbox
{
    public class OutboxOptions
    {
        public string CollectionName { get; set; } = "Outbox";
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; } = "OutboxDb";
        public bool DeleteAfter { get; set; }
        public EventBusTypeEnum EventBusType { get; set; } = EventBusTypeEnum.RabbitMQ;
    }

    public enum EventBusTypeEnum
    {
        RabbitMQ = 1,
        Kafka = 2
    }
}
