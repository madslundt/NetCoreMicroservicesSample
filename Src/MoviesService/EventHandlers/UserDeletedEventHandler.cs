using Events.Users;
using Infrastructure.Core.Commands;
using Infrastructure.Core.Events;
using MoviesService.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace MoviesService.EventHandlers
{
    public class UserDeletedEventHandler : IEventHandler<UserDeletedEvent>
    {
        private readonly ICommandBus _commandBus;

        public UserDeletedEventHandler(ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }

        public async Task Handle(UserDeletedEvent @event, CancellationToken cancellationToken)
        {
            var command = new DeleteMoviesByUserIdCommand.Command
            {
                UserId = @event.UserId
            };

            await _commandBus.Send(command, cancellationToken);
        }
    }
}
