using System;

namespace Infrastructure.Outbox
{
    public class OutboxMessage
    {
        public OutboxMessage()
        {}
        public OutboxMessage(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; } = Guid.NewGuid();
        public DateTime CreatedUtc { get; } = DateTime.UtcNow;
        public string Type { get; set; }
        public string Data { get; set; }
        public DateTime? Processed { get; set; }
    }
}
