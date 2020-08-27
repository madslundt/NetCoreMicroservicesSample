using Infrastructure.Core.Events;
using Infrastructure.EventStores.Stores;
using Infrastructure.MessageBrokers;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Infrastructure.Outbox
{
    public class OutboxListener : IOutboxListener
    {
        private readonly IStore _store;

        public OutboxListener(IStore store)
        {
            _store = store;
        }

        public virtual async Task Commit(OutboxMessage message)
        {
            await _store.Add(message);
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
