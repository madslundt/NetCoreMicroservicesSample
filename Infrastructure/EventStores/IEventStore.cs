using Infrastructure.Core.Events;
using Infrastructure.EventStores.Aggregates;
using Infrastructure.EventStores.Projection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.EventStores
{
    public interface IEventStore
    {
        void AddSnapshot(ISnapshot snapshot);

        void AddProjection(IProjection projection);

        Task AppendEvent<TStream>(Guid streamId, IEvent @event, int? expectedVersion = null, Func<StreamState, Task> action = null);

        Task<TAggregate> AggregateStream<TAggregate>(Guid streamId, int? version = null, DateTime? createdUtc = null) where TAggregate : IAggregate;
        Task<ICollection<TAggregate>> AggregateStream<TAggregate>(ICollection<Guid> ids) where TAggregate : IAggregate;

        Task<StreamState> GetEvent(Guid streamId);

        Task<IEnumerable<StreamState>> GetEvents(Guid streamId, int? version = null, DateTime? createdUtc = null);

        Task Store<TAggregate>(TAggregate aggregate, Func<StreamState, Task> action = null) where TAggregate : IAggregate;
        Task Store<TAggregate>(ICollection<TAggregate> aggregate, Func<StreamState, Task> action = null) where TAggregate : IAggregate;
    }
}
