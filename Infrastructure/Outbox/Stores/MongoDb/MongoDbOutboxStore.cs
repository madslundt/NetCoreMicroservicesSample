using Infrastructure.Outbox.Providers.MongoDb;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<OutboxMessage>> GetUnprocessedMessages()
        {
            var filter = Builders<OutboxMessage>.Filter.Where(d => !d.Processed.HasValue);
            var cursor = await _outboxMessages.Find(filter).ToCursorAsync();

            var result = cursor.ToEnumerable();

            return result;
        }

        public async Task SetMessageToProcessed(Guid id)
        {
            var filter = Builders<OutboxMessage>.Filter.Eq(d => d.Id, id);
            var update = Builders<OutboxMessage>.Update.Set(x => x.Processed, DateTime.UtcNow);

            await _outboxMessages.UpdateOneAsync(filter, update);
        }
    }
}
