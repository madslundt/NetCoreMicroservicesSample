using System;

namespace Infrastructure.EventStores
{
    public class StreamState
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid AggregateId { get; set; }
        public DateTime CreatedUtc { get; private set; } = DateTime.UtcNow;
        public string Type { get; set; }
        public string Data { get; set; }
        public int Version { get; set; } = 0;

    }
}
