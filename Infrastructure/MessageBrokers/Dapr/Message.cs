using Infrastructure.Core.Events;
using System;

namespace Infrastructure.MessageBrokers.Dapr
{
    public class Message
    {
        public Guid Id { get; set; }
        public IEvent Content { get; set; }
        public string Subject { get; set; }
    }
}
