using Events.Messages;
using Infrastructure.EventStores.Aggregates;
using System;

namespace MessagesService
{
    public class MessageAggregate : Aggregate
    {
        public Guid UserId { get; private set; }
        public string Text { get; private set; }

        protected MessageAggregate()
        { }

        private MessageAggregate(Guid userId, string text)
        {
            var @event = new MessageCreatedEvent
            {
                MessageId = Guid.NewGuid(),
                Text = text,
                UserId = userId
            };

            Enqueue(@event);
            Apply(@event);
        }

        public static MessageAggregate CreateMessage(Guid userId, string text)
        {
            return new MessageAggregate(userId, text);
        }


        private void Apply(MessageCreatedEvent @event)
        {
            Id = @event.MessageId;
            UserId = @event.UserId;
            Text = @event.Text;
        }
    }
}
