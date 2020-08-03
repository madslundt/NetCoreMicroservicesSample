using FluentValidation;
using Infrastructure.Core.Events;
using System;

namespace Events.Messages
{
    public class MessageCreatedEvent : Event
    {
        public Guid MessageId { get; set; }
        public Guid UserId { get; set; }
        public string Text { get; set; }

        public class Validator : AbstractValidator<MessageCreatedEvent>
        {
            public Validator()
            {
                RuleFor(e => e.UserId).NotEmpty();
                RuleFor(e => e.Text).NotEmpty();
            }
        }
    }
}
