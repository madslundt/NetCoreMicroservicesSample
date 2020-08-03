using Infrastructure.Core.Events;
using System;
using System.Collections.Generic;

namespace Infrastructure.EventStores.Aggregates
{
    public interface IAggregate
    {
        Guid Id { get; }
        int Version { get; }
        DateTime Created { get; }

        IEnumerable<IEvent> DequeueUncommittedEvents();

    }
}
