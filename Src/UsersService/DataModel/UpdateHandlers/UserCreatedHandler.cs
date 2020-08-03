using DataModel.Models.User;
using Events.Users;
using Infrastructure.Core;
using Infrastructure.Core.Events;
using System.Threading;
using System.Threading.Tasks;

namespace DataModel.UpdateHandlers
{
    public class UserCreatedHandler : IEventHandler<UserCreatedEvent>
    {
        private readonly DatabaseContext _db;

        public UserCreatedHandler(DatabaseContext db)
        {
            _db = db;
        }

        public async Task Handle(UserCreatedEvent @event, CancellationToken cancellationToken)
        {
            var user = Mapping.Map<UserCreatedEvent, User>(@event);
            user.Id = @event.UserId;

            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
        }
    }
}
