using System.Threading;
using System.Threading.Tasks;
using Events.Users;
using Infrastructure.Core.Events;
using MediatR;
using MessagesService.Commands;

namespace MessagesService.EventHandlers
{
    public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
    {
        private readonly IMediator _mediator;

        public UserCreatedEventHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
        {
            var text = "Welcome";
            var command = new CreateMessageCommand.Command(notification.UserId, text);

            await _mediator.Send(command);
        }
    }
}
