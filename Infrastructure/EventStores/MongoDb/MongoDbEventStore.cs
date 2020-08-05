using FluentValidation;
using Infrastructure.Core.Events;
using Infrastructure.EventStores.Aggregates;
using Infrastructure.EventStores.MongoDb;
using Infrastructure.EventStores.Projection;
using Infrastructure.MessageBrokers;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.EventStores
{
    public class MongoDbEventStore : IEventStore
    {
        private readonly IMongoCollection<StreamState> _streamStates;
        private readonly IList<ISnapshot> snapshots = new List<ISnapshot>();
        private readonly IList<IProjection> projections = new List<IProjection>();
        private readonly IValidatorFactory _validationFactory;

        public MongoDbEventStore(IOptions<MongoDbEventStoreOptions> options, IValidatorFactory validationFactory)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            var database = client.GetDatabase(options.Value.DatabaseName);
            _streamStates = CreateOrGetCollection(database, options.Value.CollectionName);
            _validationFactory = validationFactory;
        }

        private IMongoCollection<StreamState> CreateOrGetCollection(IMongoDatabase database, string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            var collections = database.ListCollections(new ListCollectionsOptions { Filter = filter });

            var doesCollectionExist = collections.Any();

            if (!doesCollectionExist)
            {
                database.CreateCollection(collectionName);
            }

            return database.GetCollection<StreamState>(collectionName);
        }

        public virtual void AddProjection(IProjection projection)
        {
            throw new NotImplementedException();
        }

        public virtual void AddSnapshot(ISnapshot snapshot)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<TAggregate> AggregateStream<TAggregate>(Guid streamId, int? version = null, DateTime? createdUtc = null) where TAggregate : IAggregate
        {
            var aggregate = (TAggregate)Activator.CreateInstance(typeof(TAggregate), true);

            var events = await GetEvents(streamId, version, createdUtc);
            var v = 0;

            foreach (var @event in events)
            {
                aggregate.InvokeIfExists("Apply", DeserializeObject(@event?.Data));
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

        public virtual async Task AppendEvent<TStream>(Guid streamId, IEvent @event, int? expectedVersion = null, Func<StreamState, Task> action = null)
        {
            var version = 1;

            var events = await GetEvents(streamId);
            var versions = events.Select(e => e.Version).ToList();

            if (expectedVersion.HasValue)
            {
                if (versions.Contains(expectedVersion.Value))
                {
                    throw new Exception($"Version '{expectedVersion.Value}' already exists for stream '{streamId}'");
                }
            }
            else
            {
                version = versions.DefaultIfEmpty(0).Max() + 1;
            }

            var stream = new StreamState
            {
                AggregateId = streamId,
                Version = version,
                Type = MessageBrokersHelper.GetTypeName(@event.GetType()),
                Data = @event == null ? "{}" : JsonConvert.SerializeObject(@event, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                })
            };

            await _streamStates.InsertOneAsync(stream);

            if (action != null)
            {
                await action(stream);
            }
        }

        private static IEvent DeserializeObject(string obj)
        {
            return JsonConvert.DeserializeObject<IEvent>(obj, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
        }

        public virtual async Task<IEnumerable<StreamState>> GetEvents(Guid streamId, int? version = null, DateTime? createdUtc = null)
        {
            var filter = Builders<StreamState>.Filter.Where(d => d.AggregateId == streamId /*&& (!version.HasValue || d.Version == version) && (!createdUtc.HasValue || d.CreatedUtc == createdUtc)*/);

            var cursor = await _streamStates
                .Find(filter)
                .ToCursorAsync();

            var result = cursor.ToEnumerable();

            return result;
        }

        public virtual async Task<StreamState> GetEvent(Guid streamId)
        {
            var cursor = await _streamStates.Find(Builders<StreamState>.Filter.Where(d => d.Id == streamId)).SortByDescending(d => d.Version).ToCursorAsync();

            var result = await cursor.FirstOrDefaultAsync();

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

        public virtual async Task Store<TAggregate>(ICollection<TAggregate> aggregates, Func<StreamState, Task> action = null) where TAggregate : IAggregate
        {
            foreach (var aggregate in aggregates)
            {
                await Store(aggregate, action);
            }
        }
    }
}
