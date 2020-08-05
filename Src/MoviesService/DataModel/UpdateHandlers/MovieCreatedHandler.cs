using DataModel.Models;
using Events.Movies;
using Infrastructure.Core;
using Infrastructure.Core.Events;
using System.Threading;
using System.Threading.Tasks;

namespace DataModel.UpdateHandlers
{
    public class MovieCreatedHandler : IEventHandler<MovieCreatedEvent>
    {
        private readonly DatabaseContext _db;

        public MovieCreatedHandler(DatabaseContext db)
        {
            _db = db;
        }

        public async Task Handle(MovieCreatedEvent @event, CancellationToken cancellationToken)
        {
            var movie = Mapping.Map<MovieCreatedEvent, Movie>(@event);
            movie.Id = @event.MovieId;

            await _db.AddAsync(movie);
            await _db.SaveChangesAsync();
        }
    }
}
