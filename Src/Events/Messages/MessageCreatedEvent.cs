using FluentValidation;
using Infrastructure.EventBus;
using System;

namespace Events.Messages
{
    public class MessageCreatedEvent : IEvent
    {
        public Guid MessageId { get; }

        public MessageCreatedEvent(Guid messageId)
        {
            MessageId = messageId;
        }

        public class Validator : AbstractValidator<MessageCreatedEvent>
        {
            public Validator()
            {
                RuleFor(cmd => cmd.MessageId).Empty();
            }
        }
    }
}
