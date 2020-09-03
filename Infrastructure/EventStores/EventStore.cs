using FluentValidation;
using Infrastructure.Core.Events;
using Infrastructure.EventStores.Aggregate;
using Infrastructure.EventStores.Projection;
using Infrastructure.EventStores.Snapshot;
using Infrastructure.EventStores.Stores;
using Infrastructure.MessageBrokers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.EventStores
{
    public class EventStore : IEventStore
    {
        private readonly IList<ISnapshot> _snapshots = new List<ISnapshot>();
        private readonly IList<IProjection> _projections = new List<IProjection>();
        private readonly IStore _store;
        private readonly IValidatorFactory _validationFactory;

        private readonly JsonSerializerSettings JSON_SERIALIZER_SETTINGS = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public EventStore(IStore store, IValidatorFactory validationFactory)
        {
            _store = store;
            _validationFactory = validationFactory;
        }

        public virtual void AddProjection(IProjection projection)
        {
            _projections.Add(projection);
        }

        public virtual void AddSnapshot(ISnapshot snapshot)
        {
            _snapshots.Add(snapshot);
        }

        public virtual async Task<TAggregate> AggregateStream<TAggregate>(Guid aggregateId, int? version = null, DateTime? createdUtc = null) where TAggregate : IAggregate
        {
            var aggregate = (TAggregate)Activator.CreateInstance(typeof(TAggregate), true);

            var events = await GetEvents(aggregateId, version, createdUtc);
            var v = 0;

            foreach (var @event in events)
            {
                aggregate.InvokeIfExists("Apply", JsonConvert.DeserializeObject<IEvent>(@event?.Data, JSON_SERIALIZER_SETTINGS));
                aggregate.SetIfExists(nameof(IAggregate.Version), ++v);
                aggregate.SetIfExists(nameof(IAggregate.CreatedUtc), @event.CreatedUtc);
            }

            return aggregate;
        }

        public virtual async Task<ICollection<TAggregate>> AggregateStream<TAggregate>(ICollection<Guid> ids) where TAggregate : IAggregate
        {
            var aggregates = new List<TAggregate>();
            foreach (var id in ids)
            {
                var aggregate = await AggregateStream<TAggregate>(id);
                aggregates.Add(aggregate);
            }

            return aggregates;
        }

        public virtual async Task AppendEvent<TAggregate>(Guid aggregateId, IEvent @event, int? expectedVersion = null, Func<StreamState, Task> action = null)
            where TAggregate : IAggregate
        {
            var version = 1;

            var events = await GetEvents(aggregateId);
            var versions = events.Select(e => e.Version).ToList();

            if (expectedVersion.HasValue)
            {
                if (versions.Contains(expectedVersion.Value))
                {
                    throw new Exception($"Version '{expectedVersion.Value}' already exists for stream '{aggregateId}'");
                }
            }
            else
            {
                version = versions.DefaultIfEmpty(0).Max() + 1;
            }

            var stream = new StreamState
            {
                AggregateId = aggregateId,
                Version = version,
                Type = MessageBrokersHelper.GetTypeName(@event.GetType()),
                Data = @event == null ? "{}" : JsonConvert.SerializeObject(@event, JSON_SERIALIZER_SETTINGS)
            };

            await _store.Add(stream);

            if (action != null)
            {
                await action(stream);
            }
        }

        public virtual async Task<IEnumerable<StreamState>> GetEvents(Guid aggregateId, int? version = null, DateTime? createdUtc = null)
        {
            var result = await _store.GetEvents(aggregateId, version, createdUtc);

            return result;
        }

        public virtual async Task Store<TAggregate>(TAggregate aggregate, Func<StreamState, Task> action = null) where TAggregate : IAggregate
        {
            var events = aggregate.DequeueUncommittedEvents();
            var initialVersion = aggregate.Version - events.Count();

            foreach (var @event in events)
            {
                initialVersion++;

                var validator = _validationFactory.GetValidator(@event.GetType());
                var result = validator?.Validate(new ValidationContext<IEvent>(@event));
                if (result != null && !result.IsValid)
                {
                    continue;
                }

                await AppendEvent<TAggregate>(aggregate.Id, @event, initialVersion, action);

                var projections = _projections.Where(projection => projection.Handles.Contains(@event.GetType()));
                foreach (var projection in projections)
                {
                    projection.Handle(@event);
                }
            }

            _snapshots
                .FirstOrDefault(snapshot => snapshot.Handles == typeof(TAggregate))?
                .Handle(aggregate);
        }

        public virtual async Task Store<TAggregate>(ICollection<TAggregate> aggregates, Func<StreamState, Task> action = null) where TAggregate : IAggregate
        {
            foreach (var aggregate in aggregates)
            {
                await Store(aggregate, action);
            }
        }
    }
}
