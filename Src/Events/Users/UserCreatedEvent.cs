using FluentValidation;
using Infrastructure.EventBus;
using System;

namespace Events.Users
{
    public class UserCreatedEvent : IEvent
    {
        public Guid UserId { get; }

        public UserCreatedEvent(Guid userId)
        {
            UserId = userId;
        }

        public class Validator : AbstractValidator<UserCreatedEvent>
        {
            public Validator()
            {
                RuleFor(cmd => cmd.UserId).Empty();
            }
        }
    }
}
