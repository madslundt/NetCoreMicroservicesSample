using FluentValidation;
using Infrastructure.Core.Events;
using System;

namespace Events.Messages
{
    public class MessageCreatedEvent : IEvent
    {
        public Guid MessageId { get; set; }
        public Guid UserId { get; set; }
        public string Text { get; set; }

        public class Validator : AbstractValidator<MessageCreatedEvent>
        {
            public Validator()
            {
                RuleFor(cmd => cmd.UserId).Empty();
                RuleFor(cmd => cmd.Text).Empty();
            }
        }
    }
}
