using Events.Movies;
using Infrastructure.Core.Commands;
using Infrastructure.Core.Events;
using ReviewsService.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace ReviewsService.EventHandlers
{
    public class MovieDeletedEventHandler : IEventHandler<MovieDeletedEvent>
    {
        private readonly ICommandBus _commandBus;

        public MovieDeletedEventHandler(ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }

        public async Task Handle(MovieDeletedEvent @event, CancellationToken cancellationToken)
        {
            var command = new DeleteReviewsByMovieIdCommand.Command
            {
                MovieId = @event.MovieId
            };

            await _commandBus.Send(command, cancellationToken);
        }
    }
}
