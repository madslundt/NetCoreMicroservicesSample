using System;

namespace Infrastructure.EventStores
{
    public class StreamState
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedUtc { get; } = DateTime.UtcNow;
        public string Type { get; set; }
        public string Data { get; set; }
        public int Version { get; set; } = 0;

    }
}
