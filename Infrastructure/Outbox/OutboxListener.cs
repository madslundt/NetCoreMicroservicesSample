using Infrastructure.EventBus;
using Infrastructure.EventBus.RabbitMQ;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Infrastructure.Outbox
{
    public interface IOutboxListener
    {
        Task Commit<T>(T message);
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

        public async Task Commit<T>(T message)
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
