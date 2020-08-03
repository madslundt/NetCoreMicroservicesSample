using FluentValidation;
using Infrastructure.Core.Events;
using System;

namespace Events.Movies
{
    public class MovieCreatedEvent : Event
    {
        public Guid MovieId { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }

        public class Validator : AbstractValidator<MovieCreatedEvent>
        {
            public Validator()
            {
                RuleFor(e => e.MovieId).NotEmpty();
                RuleFor(e => e.UserId).NotEmpty();
                RuleFor(e => e.Title).NotEmpty();
                RuleFor(e => e.Year).GreaterThanOrEqualTo(1900);
            }
        }
    }
}
