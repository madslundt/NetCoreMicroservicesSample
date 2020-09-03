using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.EventStores.Stores.MongoDb
{
    public class MongoDbEventStore : IStore
    {
        private readonly IMongoCollection<StreamState> _streamStates;

        public MongoDbEventStore(IOptions<MongoDbEventStoreOptions> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            var database = client.GetDatabase(options.Value.DatabaseName);
            _streamStates = CreateOrGetCollection(database, options.Value.CollectionName);
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

        public async Task Add(StreamState stream)
        {
            await _streamStates.InsertOneAsync(stream);
        }

        public async Task<IEnumerable<StreamState>> GetEvents(Guid aggregateId, int? version = null, DateTime? createdUtc = null)
        {
            var builder = Builders<StreamState>.Filter;
            var filter = builder.Where(d => d.AggregateId == aggregateId);

            if (version.HasValue)
            {
                filter = filter & builder.Where(d => d.Version == version.Value);
            }
            if (createdUtc.HasValue)
            {
                filter = filter & builder.Where(d => d.CreatedUtc == createdUtc);
            }

            var cursor = await _streamStates
                .Find(filter)
                .SortBy(s => s.Version)
                .ToCursorAsync();

            var result = cursor.ToEnumerable();

            return result;
        }
    }
}
