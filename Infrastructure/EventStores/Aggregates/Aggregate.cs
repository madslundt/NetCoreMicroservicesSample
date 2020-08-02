using Infrastructure.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.EventStores.Aggregates
{
    public abstract class Aggregate : IAggregate
    {
        public Guid AggregateId { get; protected set; } = Guid.NewGuid();
        public int Version { get; protected set; } = 0;
        public virtual string Name => "";

        [NonSerialized]
        private readonly List<IEvent> uncommittedEvents = new List<IEvent>();

        protected Aggregate()
        { }

        IEnumerable<IEvent> IAggregate.DequeueUncommittedEvents()
        {
            var dequeuedEvents = uncommittedEvents.ToList();

            uncommittedEvents.Clear();

            return dequeuedEvents;
        }

        protected virtual void Enqueue(IEvent @event)
        {
            Version++;
            uncommittedEvents.Add(@event);
        }
    }
}
