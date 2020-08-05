using FluentValidation;
using Infrastructure.Core.Events;
using System;

namespace Events.Reviews
{
    public class ReviewCreatedEvent : Event
    {
        public Guid ReviewId { get; set; }
        public Guid MovieId { get; set; }
        public Guid UserId { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; }

        public class Validator : AbstractValidator<ReviewCreatedEvent>
        {
            public Validator()
            {
                RuleFor(e => e.ReviewId).NotEmpty();
                RuleFor(e => e.MovieId).NotEmpty();
                RuleFor(e => e.UserId).NotEmpty();
                RuleFor(e => e.Text).NotEmpty();
                RuleFor(e => e.Rating).GreaterThanOrEqualTo(1).LessThanOrEqualTo(5);
            }
        }
    }
}
