using Events.Users;
using Infrastructure.Core.Events;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataModel.UpdateHandlers
{
    public class UserDeletedHandler : IEventHandler<UserDeletedEvent>
    {
        private readonly DatabaseContext _db;

        public UserDeletedHandler(DatabaseContext db)
        {
            _db = db;
        }

        public async Task Handle(UserDeletedEvent @event, CancellationToken cancellationToken)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == @event.UserId);

            if (user is null)
            {
                throw new ArgumentNullException($"Could not find {nameof(user)} with id '{@event.UserId}'");
            }

            _db.Remove(user);
            await _db.SaveChangesAsync();
        }
    }
}
