using FluentValidation;
using Infrastructure.Core.Events;
using System;

namespace Events.Users
{
    public class UserDeletedEvent : Event
    {
        public Guid UserId { get; set; }

        public class Validator : AbstractValidator<UserDeletedEvent>
        {
            public Validator()
            {
                RuleFor(e => e.UserId).NotEmpty();
            }
        }
    }
}
