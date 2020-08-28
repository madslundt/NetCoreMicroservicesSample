using Infrastructure.EventStores.Aggregate;
using System;

namespace Infrastructure.EventStores.Snapshot
{
    public interface ISnapshot
    {
        Type Handles { get; }
        void Handle(IAggregate aggregate);
    }
}
