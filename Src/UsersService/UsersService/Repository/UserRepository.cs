using DataModel;
using DataModel.Models.User;
using Events.Users;
using Infrastructure.Outbox;
using System.Threading;
using System.Threading.Tasks;

namespace UsersService.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IOutboxListener _outboxListener;
        private readonly DatabaseContext _db;

        public UserRepository(IOutboxListener outboxListener, DatabaseContext db)
        {
            _outboxListener = outboxListener;
            _db = db;
        }

        public async Task CreateUser(User user, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var transaction = await _db.Database.BeginTransactionAsync(cancellationToken))
            {
                _db.Add(user);
                await _db.SaveChangesAsync();

                var @event = new UserCreatedEvent(user.Id);
                await _outboxListener.Commit(@event);

                await transaction.CommitAsync(cancellationToken);
            }
        }
    }
}
