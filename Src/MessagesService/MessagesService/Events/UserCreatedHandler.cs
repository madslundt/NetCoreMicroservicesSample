using System.Threading;
using System.Threading.Tasks;
using Events.Users;
using Infrastructure.RabbitMQ;
using MediatR;
using MessagesService.Commands;

namespace MessagesService.Events
{
    public class UserCreatedHandler : IEventHandler<UserCreated>
    {
        private readonly IMediator _mediator;

        public UserCreatedHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task Handle(UserCreated notification, CancellationToken cancellationToken)
        {
            var text = "Welcome";
            var command = new CreateMessage.Command(notification.UserId, text);

            await _mediator.Send(command);
        }
    }
}
