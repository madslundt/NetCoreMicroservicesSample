using Infrastructure.MessageBrokers;
using Infrastructure.Outbox.Stores;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Outbox
{
    internal sealed class OutboxProcessor : IHostedService
    {
        private readonly IEventListener _eventListener;
        private readonly IStore _store;
        private readonly OutboxOptions _outboxOptions;
        private Timer _timer;

        public OutboxProcessor(IEventListener eventListener, IOptions<OutboxOptions> options, IStore store)
        {
            _eventListener = eventListener;
            _store = store;
            _outboxOptions = options.Value;

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(SendOutboxMessages, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void SendOutboxMessages(object state)
        {
            _ = Process();
        }

        public async Task Process()
        {
            var messages = await _store.GetUnprocessedMessages();
            var publishedMessageIds = new List<Guid>();
            try
            {
                foreach (var message in messages)
                {
                    await _eventListener.Publish(message.Data, message.Type);
                    publishedMessageIds.Add(message.Id);
                    await _store.SetMessageToProcessed(message.Id);
                }
            }
            finally
            {
                if (_outboxOptions.DeleteAfter)
                {
                    await _store.Delete(publishedMessageIds);
                }
            }
        }
    }
}
