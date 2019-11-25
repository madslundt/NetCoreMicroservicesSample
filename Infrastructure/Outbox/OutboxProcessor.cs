using Infrastructure.RabbitMQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Outbox
{
    internal sealed class OutboxProcessor : IHostedService
    {
        private readonly IMongoCollection<OutboxMessage> _outboxMessages;
        private readonly IRabbitMQListener _rabbitMqListener;
        private readonly OutboxOptions _outboxOptions;
        private Timer _timer;

        public OutboxProcessor(IRabbitMQListener rabbitMqListener, IOptions<OutboxOptions> options)
        {
            _rabbitMqListener = rabbitMqListener;
            _outboxOptions = options.Value;

            var client = new MongoClient(options.Value.ConnectionString);
            var database = client.GetDatabase(options.Value.DatabaseName);
            _outboxMessages = database.GetCollection<OutboxMessage>(options.Value.CollectionName);
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
            var cursor = await _outboxMessages.Find(Builders<OutboxMessage>.Filter.Where(d => !d.Processed.HasValue)).ToCursorAsync();
            var publishedMessages = new List<(Guid id, DateTime processed)>();
            try
            {
                foreach (var message in cursor.ToEnumerable())
                {
                    await _rabbitMqListener.Publish(message.Data, message.Type);
                    publishedMessages.Add((id: message.Id, processed: DateTime.UtcNow));
                }
            }
            finally
            {
                if (_outboxOptions.DeleteAfter)
                {
                    var ids = publishedMessages.Select(message => message.id);
                    await _outboxMessages.DeleteManyAsync(Builders<OutboxMessage>.Filter.In(d => d.Id, ids));
                }
                else
                {
                    foreach (var publishedMessage in publishedMessages)
                    {
                        await _outboxMessages.UpdateOneAsync(Builders<OutboxMessage>.Filter.Eq(d => d.Id, publishedMessage.id), Builders<OutboxMessage>.Update.Set(x => x.Processed, publishedMessage.processed));
                    }
                }
            }
        }
    }
}
