using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Outbox
{
    public class OutboxMessage
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public DateTime Created { get; private set; } = DateTime.UtcNow;
        public string Type { get; private set; }
        public string Data { get; private set; }

        private OutboxMessage()
        { }

        internal OutboxMessage(DateTime created, string type, string data)
        {
            Created = created;
            Type = type;
            Data = data;
        }
    }
}
