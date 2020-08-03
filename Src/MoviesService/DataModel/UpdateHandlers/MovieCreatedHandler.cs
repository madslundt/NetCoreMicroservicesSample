using DataModel.Models;
using Events.Movies;
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
            var movie = new Movie
            {
                Id = @event.MovieId,
                UserId = @event.UserId,
                Title = @event.Title,
                Year = @event.Year,
            };

            await _db.AddAsync(movie);
            await _db.SaveChangesAsync();
        }
    }
}
