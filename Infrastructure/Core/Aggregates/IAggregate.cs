using Infrastructure.Core.Events;
using System;
using System.Collections.Generic;

namespace Infrastructure.Core.Aggregates
{
    public interface IAggregate
    {
        Guid AggregateId { get; }
        int Version { get; }
        IEnumerable<IEvent> DequeueUncommittedEvents();

    }
}
