namespace Infrastructure.Outbox.Stores.Dapr
{
    public class DaprOutboxOptions : OutboxOptions
    {
        public string Uri { get; set; }
    }
}
