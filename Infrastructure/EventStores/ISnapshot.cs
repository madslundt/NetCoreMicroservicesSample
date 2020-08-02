using Infrastructure.EventStores.Aggregates;
using System;

namespace Infrastructure.EventStores
{
    public interface ISnapshot
    {
        Type Handles { get; }
        void Handle(IAggregate aggregate);
    }
}
