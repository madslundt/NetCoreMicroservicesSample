namespace Infrastructure.Outbox
{
    public class OutboxOptions
    {
        public string CollectionName { get; set; } = "Outbox";
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; } = "OutboxDb";
        public bool DeleteAfter { get; set; }
    }
}
