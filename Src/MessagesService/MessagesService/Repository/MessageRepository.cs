using DataModel;
using DataModel.Models.Message;
using Events.Messages;
using Infrastructure.Outbox;
using System.Threading;
using System.Threading.Tasks;

namespace MessagesService.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IOutboxListener _outboxListener;
        private readonly DatabaseContext _db;

        public MessageRepository(IOutboxListener outboxListener, DatabaseContext db)
        {
            _outboxListener = outboxListener;
            _db = db;
        }

        public async Task CreateMessage(Message message, CancellationToken cancellationToken)
        {
            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                _db.Add(message);
                await _db.SaveChangesAsync();

                var @event = new MessageCreatedEvent(message.Id);
                await _outboxListener.Commit(@event);

                await transaction.CommitAsync(cancellationToken);
            }
        }
    }
}
