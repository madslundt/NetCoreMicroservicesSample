using Infrastructure.Core.Events;
using Infrastructure.EventStores.Aggregates;
using Infrastructure.EventStores.Projection;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Infrastructure.MessageBrokers;
using Newtonsoft.Json;
using FluentValidation;

namespace Infrastructure.EventStores.Providers.EfCore
{
    public class EfCoreEventStore : IEventStore
    {
        private readonly EventStoreContext _context;
        private readonly IList<ISnapshot> snapshots = new List<ISnapshot>();
        private readonly IList<IProjection> projections = new List<IProjection>();
        private readonly IValidatorFactory _validationFactory;

        private readonly JsonSerializerSettings JSON_SERIALIZER_SETTINGS = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public EfCoreEventStore(EventStoreContext context, IValidatorFactory validationFactory)
        {
            _context = context;
            _validationFactory = validationFactory;

            _context.Database.EnsureCreated();
        }

        public void AddProjection(IProjection projection)
        {
            throw new NotImplementedException();
        }

        public void AddSnapshot(ISnapshot snapshot)
        {
            throw new NotImplementedException();
        }

        public async Task<TAggregate> AggregateStream<TAggregate>(Guid aggregateId, int? version = null, DateTime? createdUtc = null) where TAggregate : IAggregate
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

        public async Task<ICollection<TAggregate>> AggregateStream<TAggregate>(ICollection<Guid> ids) where TAggregate : IAggregate
        {
            var aggregates = new List<TAggregate>();
            foreach (var id in ids)
            {
                var aggregate = await AggregateStream<TAggregate>(id);
                aggregates.Add(aggregate);
            }

            return aggregates;
        }

        public async Task AppendEvent<TStream>(Guid aggregateId, IEvent @event, int? expectedVersion = null, Func<StreamState, Task> action = null)
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

            await _context.AddAsync(stream);
            await _context.SaveChangesAsync();

            if (action != null)
            {
                await action(stream);
            }
        }

        public async Task<IEnumerable<StreamState>> GetEvents(Guid streamId, int? version = null, DateTime? createdUtc = null)
        {
            var query = from stream in _context.Streams
                        where stream.AggregateId == streamId
                        select stream;

            if (version.HasValue)
            {
                query.Where(q => q.Version == version);
            }
            if (createdUtc.HasValue)
            {
                query.Where(q => q.CreatedUtc == createdUtc);
            }

            var result = await query.ToListAsync();

            return result;
        }

        public async Task<StreamState> GetStream(Guid streamId)
        {
            var query = from stream in _context.Streams
                        where stream.Id == streamId
                        select stream;

            var result = await query.FirstOrDefaultAsync();

            return result;
        }

        public async Task Store<TAggregate>(TAggregate aggregate, Func<StreamState, Task> action = null) where TAggregate : IAggregate
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

                foreach (var projection in projections.Where(
                    projection => projection.Handles.Contains(@event.GetType())))
                {
                    projection.Handle(@event);
                }
            }

            snapshots
                .FirstOrDefault(snapshot => snapshot.Handles == typeof(TAggregate))?
                .Handle(aggregate);
        }

        public async Task Store<TAggregate>(ICollection<TAggregate> aggregates, Func<StreamState, Task> action = null) where TAggregate : IAggregate
        {
            foreach (var aggregate in aggregates)
            {
                await Store(aggregate, action);
            }
        }
    }
}
