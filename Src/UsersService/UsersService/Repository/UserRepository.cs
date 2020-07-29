using DataModel;
using DataModel.Models.User;
using Events.Users;
using Infrastructure.Core;
using Infrastructure.Outbox;
using System.Threading;
using System.Threading.Tasks;

namespace UsersService.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IOutboxListener _outboxListener;
        private readonly DatabaseContext _db;
        private readonly TransactionId _transactionId;

        public UserRepository(IOutboxListener outboxListener, DatabaseContext db, TransactionId transactionId)
        {
            _outboxListener = outboxListener;
            _db = db;
            _transactionId = transactionId;
        }

        public async Task CreateUser(User user, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var transaction = await _db.Database.BeginTransactionAsync(cancellationToken))
            {
                _db.Add(user);
                await _db.SaveChangesAsync();

                var @event = new UserCreatedEvent(user.Id);
                await _outboxListener?.Add(@event, _transactionId.Value);

                await transaction.CommitAsync(cancellationToken);
            }
        }
    }
}
