using Infrastructure.EventBus;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Outbox
{
    public interface IOutboxListener
    {
        Task<Guid> Add<T>(T message, Guid transactionId) where T : IEvent;
        Task Remove(Guid transactionId);
        Task Commit(Guid transactionId);
        Task Commit<T>(T message) where T : IEvent;
    }
    
    public class OutboxListener : IOutboxListener
    {
        private readonly IMongoCollection<OutboxMessage> _outboxMessages;

        public OutboxListener(IOptions<OutboxOptions> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            var database = client.GetDatabase(options.Value.DatabaseName);
            _outboxMessages = database.GetCollection<OutboxMessage>(options.Value.CollectionName);
        }

        public async Task<Guid> Add<T>(T message, Guid transactionId) where T : IEvent
        {
            var outboxMessage = new OutboxMessage
            {
                Type = EventBusHelper.GetTypeName<T>(),
                Data = message == null ? "{}" : JsonConvert.SerializeObject(message, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                }),
                TransactionId = transactionId
            };

            await _outboxMessages.InsertOneAsync(outboxMessage);

            return outboxMessage.Id;
        }

        public async Task Commit(Guid transactionId)
        {
            var filter = Builders<OutboxMessage>.Filter.Eq(x => x.TransactionId, transactionId);
            var update = Builders<OutboxMessage>.Update.Set(x => x.TransactionId, null);

            await _outboxMessages.UpdateManyAsync(filter, update);
        }

        public async Task Remove(Guid transactionId)
        {
            var filter = Builders<OutboxMessage>.Filter.Eq(x => x.TransactionId, transactionId);
            await _outboxMessages.DeleteManyAsync(filter);
        }

        public async Task Commit<T>(T message) where T : IEvent
        {
            var outboxMessage = new OutboxMessage
            {
                Type = EventBusHelper.GetTypeName<T>(),
                Data = message == null ? "{}" : JsonConvert.SerializeObject(message, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                })
            };

            await _outboxMessages.InsertOneAsync(outboxMessage);
        }
    }
}
