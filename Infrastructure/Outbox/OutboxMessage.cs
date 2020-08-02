using Infrastructure.EventStores;
using System;

namespace Infrastructure.Outbox
{
    public class OutboxMessage : StreamState
    {
        public DateTime? Processed { get; set; }
    }
}
