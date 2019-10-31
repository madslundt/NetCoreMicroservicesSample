using Convey.CQRS.Events;
using Convey.MessageBrokers;
using Events;
using System;
using System.Threading.Tasks;

namespace MessagesService.Events
{
    public class UserCreated
    {
        [Message("UserCreated")]
        public class UserCreatedEvent : IUserCreated
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
