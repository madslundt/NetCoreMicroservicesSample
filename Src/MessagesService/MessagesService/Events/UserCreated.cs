using Convey.CQRS.Events;
using Convey.MessageBrokers;
using System;
using System.Threading.Tasks;

namespace MessagesService.Events
{
    public class UserCreated
    {
        [Message("user_created")]
        public class UserCreatedEvent : IEvent
        {
            public Guid UserId { get; }

            public UserCreatedEvent(Guid userId)
            {
                UserId = userId;
            }
        }

        public class UserCreatedHandler : IEventHandler<UserCreatedEvent>
        {
            public Task HandleAsync(UserCreatedEvent @event)
            {
                Console.WriteLine($"Creating default message for {nameof(@event.UserId)} = {@event.UserId}");

                return Task.CompletedTask;
            }
        }
    }
}
