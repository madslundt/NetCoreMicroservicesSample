using Events.Users;
using Infrastructure.EventBus;
using System.Threading;
using System.Threading.Tasks;

namespace UsersService.EventHandlers
{
    public class UserCreatedHandler
    {
        public class Handler : IEventHandler<UserCreatedEvent>
        {
            public Task Handle(UserCreatedEvent @event, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}
