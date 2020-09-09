using Infrastructure.MessageBrokers;
using Infrastructure.Outbox.Stores;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly OutboxOptions _outboxOptions;
        private readonly IEventListener _eventListener;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private Timer _timer;

        public OutboxProcessor(IServiceScopeFactory serviceScopeFactory, IOptions<OutboxOptions> options, IEventListener eventListener)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _eventListener = eventListener;
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
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var store = scope.ServiceProvider.GetRequiredService<IOutboxStore>();
                var messageIds = await store.GetUnprocessedMessageIds();
                var publishedMessageIds = new List<Guid>();
                try
                {
                    foreach (var messageId in messageIds)
                    {
                        var message = await store.GetMessage(messageId);
                        if (message is null || message.Processed.HasValue)
                        {
                            continue;
                        }

                        await _eventListener.Publish(message.Data, message.Type);
                        await store.SetMessageToProcessed(message.Id);
                        publishedMessageIds.Add(message.Id);
                    }
                }
                finally
                {
                    if (_outboxOptions.DeleteAfter)
                    {
                        await store.Delete(publishedMessageIds);
                    }
                }
            }
        }
    }
}
