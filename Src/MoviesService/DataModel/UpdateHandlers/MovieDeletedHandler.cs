using Events.Movies;
using Infrastructure.Core.Events;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataModel.UpdateHandlers
{
    public class MovieDeletedHandler : IEventHandler<MovieDeletedEvent>
    {
        private readonly DatabaseContext _db;

        public MovieDeletedHandler(DatabaseContext db)
        {
            _db = db;
        }

        public async Task Handle(MovieDeletedEvent @event, CancellationToken cancellationToken)
        {
            var movie = await _db.Movies.FirstOrDefaultAsync(m => m.Id == @event.MovieId);

            if (movie is null)
            {
                throw new ArgumentNullException($"Could not find {nameof(movie)} with id '{@event.MovieId}'");
            }

            _db.Remove(movie);
            await _db.SaveChangesAsync();
        }
    }
}
