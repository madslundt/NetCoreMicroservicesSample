using Events.Users;
using Infrastructure.EventBus;
using System.Threading;
using System.Threading.Tasks;

namespace UsersService.Events
{
    public class UserCreatedHandler
    {
        public class Handler : IEventHandler<UserCreated>
        {
            public Task Handle(UserCreated @event, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}
