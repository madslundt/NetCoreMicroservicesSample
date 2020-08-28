using DataModel.Models.Rating;
using Events.Reviews;
using Infrastructure.EventStores.Aggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReviewsService
{
    public class ReviewAggregate : Aggregate
    {
        public Guid MovieId { get; private set; }
        public Guid UserId { get; private set; }
        public string Text { get; private set; }
        public RatingEnum Rating { get; private set; }

        protected ReviewAggregate()
        { }

        private ReviewAggregate(Guid userId, Guid movieId, string text, RatingEnum rating)
        {
            var @event = new ReviewCreatedEvent
            {
                ReviewId = Guid.NewGuid(),
                MovieId = movieId,
                UserId = userId,
                Text = text,
                Rating = (int) rating
            };

            Enqueue(@event);
            Apply(@event);
        }

        public static ReviewAggregate CreateReview(Guid userId, Guid movieId, string text, RatingEnum rating)
        {
            return new ReviewAggregate(userId, movieId, text, rating);
        }

        public void DeleteReview()
        {
            var @event = new ReviewDeletedEvent
            {
                ReviewId = Id
            };

            Enqueue(@event);
        }

        private void Apply(ReviewCreatedEvent @event)
        {
            Id = @event.ReviewId;
            MovieId = @event.MovieId;
            UserId = @event.UserId;
            Text = @event.Text;
            Rating = (RatingEnum)@event.Rating;
        }
    }
}
