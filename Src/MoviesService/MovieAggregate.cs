using Events.Movies;
using Infrastructure.EventStores.Aggregate;
using System;

namespace MoviesService
{
    public class MovieAggregate : Aggregate
    {
        public Guid UserId { get; private set; }
        public string Title { get; private set; }
        public int Year { get; private set; }

        protected MovieAggregate()
        {

        }

        private MovieAggregate(string title, int year, Guid userId)
        {
            var @event = new MovieCreatedEvent
            {
                MovieId = Guid.NewGuid(),
                UserId = userId,
                Title = title,
                Year = year,
            };

            Enqueue(@event);
            Apply(@event);
        }

        public static MovieAggregate CreateMovie(string title, int year, Guid userId)
        {
            return new MovieAggregate(title, year, userId);
        }

        public void DeleteMovie(Guid userId)
        {
            var @event = new MovieDeletedEvent
            {
                MovieId = Id,
                UserId = userId
            };

            Enqueue(@event);
        }

        private void Apply(MovieCreatedEvent @event)
        {
            Id = @event.MovieId;
            UserId = @event.UserId;
            Title = @event.Title;
            Year = @event.Year;
        }
    }
}
