using Convey.CQRS.Events;
using Convey.MessageBrokers;
using Events;
using System;
using System.Threading.Tasks;

namespace Users.Service.Events
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
                Console.WriteLine($"User created {nameof(@event.UserId)} = {@event.UserId}");

                return Task.CompletedTask;
            }
        }
    }
}
