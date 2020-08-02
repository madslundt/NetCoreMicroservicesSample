using Infrastructure.Core.Events;
using Infrastructure.MessageBrokers;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Infrastructure.Outbox
{    
    public class OutboxListener : IOutboxListener
    {
        private readonly IMongoCollection<OutboxMessage> _outboxMessages;

        public OutboxListener(IOptions<OutboxOptions> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            var database = client.GetDatabase(options.Value.DatabaseName);
            _outboxMessages = database.GetCollection<OutboxMessage>(options.Value.CollectionName);
        }

        public virtual async Task Commit(OutboxMessage message)
        {
            await _outboxMessages.InsertOneAsync(message);
        }

        public virtual async Task Commit<TEvent>(TEvent @event) where TEvent : IEvent
        {
            var outboxMessage = new OutboxMessage
            {
                Type = MessageBrokersHelper.GetTypeName<TEvent>(),
                Data = @event == null ? "{}" : JsonConvert.SerializeObject(@event, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                })
            };

            await Commit(outboxMessage);
        }
    }
}
