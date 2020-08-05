using FluentValidation;
using Infrastructure.Core.Events;
using System;

namespace Events.Reviews
{
    public class ReviewDeletedEvent : Event
    {
        public Guid ReviewId { get; set; }

        public class Validator : AbstractValidator<ReviewDeletedEvent>
        {
            public Validator()
            {
                RuleFor(e => e.ReviewId).NotEmpty();
            }
        }
    }
}
