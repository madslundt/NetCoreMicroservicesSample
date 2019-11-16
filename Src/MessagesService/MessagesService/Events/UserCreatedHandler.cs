using System.Threading;
using System.Threading.Tasks;
using Events.Infrastructure.Event;
using Events.Users;

namespace MessagesService.Events
{
    public class UserCreatedHandler : IEventHandler<UserCreated>
    {
        public Task Handle(UserCreated notification, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
