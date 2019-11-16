using System.Threading;
using System.Threading.Tasks;
using DataModel;
using Events.Infrastructure.Event;
using Events.Users;
using MediatR;

namespace MessagesService.Events
{
    public class UserCreatedHandler : IEventHandler<UserCreated>
    {
        private readonly IMediator _mediator;

        public UserCreatedHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public Task Handle(UserCreated notification, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
