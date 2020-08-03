using FluentValidation;
using Infrastructure.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Events.Movies
{
    public class MovieDeletedEvent : Event
    {
        public Guid MovieId { get; set; }
        public Guid UserId { get; set; }

        public class Validator : AbstractValidator<MovieDeletedEvent>
        {
            public Validator()
            {
                RuleFor(e => e.MovieId).NotEmpty();
                RuleFor(e => e.UserId).NotEmpty();
            }
        }
    }
}
