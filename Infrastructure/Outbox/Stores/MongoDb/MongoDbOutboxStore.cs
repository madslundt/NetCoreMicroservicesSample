using Infrastructure.Outbox.Providers.MongoDb;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Outbox.Stores.MongoDb
{
    public class MongoDbOutboxStore : IOutboxStore
    {
        private readonly IMongoCollection<OutboxMessage> _outboxMessages;

        public MongoDbOutboxStore(IOptions<MongoDbOutboxOptions> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            var database = client.GetDatabase(options.Value.DatabaseName);
            _outboxMessages = database.GetCollection<OutboxMessage>(options.Value.CollectionName);
        }

        public async Task Add(OutboxMessage message)
        {
            await _outboxMessages.InsertOneAsync(message);
        }

        public async Task Delete(IEnumerable<Guid> ids)
        {
            var filter = Builders<OutboxMessage>.Filter.In(d => d.Id, ids);
            await _outboxMessages.DeleteManyAsync(filter);
        }

        public async Task<OutboxMessage> GetMessage(Guid id)
        {
            var filter = Builders<OutboxMessage>.Filter.Where(d => d.Id == id);
            var result = await _outboxMessages.Find(filter).FirstOrDefaultAsync();

            return result;
        }

        public async Task<IEnumerable<Guid>> GetUnprocessedMessageIds()
        {
            var filter = Builders<OutboxMessage>.Filter.Where(d => !d.Processed.HasValue);
            var cursor = await _outboxMessages.Find(filter).ToCursorAsync();

            var result = cursor
                .ToList()
                .Select(c => c.Id);

            return result;
        }

        public async Task SetMessageToProcessed(Guid id)
        {
            var filter = Builders<OutboxMessage>.Filter.Where(d => d.Id == id);
            var update = Builders<OutboxMessage>.Update.Set(x => x.Processed, DateTime.UtcNow);

            var result = await _outboxMessages.UpdateOneAsync(filter, update);

            if (result.ModifiedCount == 0)
            {
                throw new Exception($"Did not modify message '{id}'");
            }
        }
    }
}
