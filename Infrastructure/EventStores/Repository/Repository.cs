using Infrastructure.Core;
using Infrastructure.Core.Aggregates;
using Infrastructure.MessageBrokers;
using Infrastructure.Outbox;
using System;
using System.Threading.Tasks;

namespace Infrastructure.EventStores.Repository
{
    public class Repository<TAggregate> : IRepository<TAggregate> where TAggregate : IAggregate
    {
        private readonly IEventStore _eventStore;
        private readonly IOutboxListener _outboxListener;
        private readonly IEventListener _eventListener;

        public Repository(IEventStore eventStore, IOutboxListener outboxListener, IEventListener eventListener)
        {
            _eventStore = eventStore;
            _outboxListener = outboxListener;
            _eventListener = eventListener;
        }
        public virtual async Task Add(TAggregate aggregate)
        {
            await _eventStore.Store(aggregate, PublishEvent);
        }

        public virtual async Task Delete(TAggregate aggregate)
        {
            await _eventStore.Store(aggregate, PublishEvent);
        }

        public virtual async Task Update(TAggregate aggregate)
        {
            await _eventStore.Store(aggregate, PublishEvent);
        }

        private async Task PublishEvent(StreamState stream)
        {
            if (stream is null)
            {
                throw new Exception($"{nameof(stream)} was null");
            }

            if (_outboxListener != null)
            {
                var message = Mapping.Map<StreamState, OutboxMessage>(stream);
                await _outboxListener.Commit(message);
            }
            else if (_eventListener != null)
            {
                await _eventListener.Publish(stream.Data, stream.Type);
            }
        }
    }
}
