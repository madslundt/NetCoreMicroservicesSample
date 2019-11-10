using Convey.CQRS.Events;
using Convey.MessageBrokers;
using System;
using System.Threading.Tasks;

namespace MessagesService.Events
{
    public class UserDeleted
    {
        [Message("UserDeleted")]
        public class UserDeletedEvent : IEvent
        {
            public Guid UserId { get; }

            public UserDeletedEvent(Guid userId)
            {
                UserId = userId;
            }
        }

        public class UserDeletedHandler : IEventHandler<UserDeletedEvent>
        {
            public Task HandleAsync(UserDeletedEvent @event)
            {
                Console.WriteLine($"Creating default message for {nameof(@event.UserId)} = {@event.UserId}");

                return Task.CompletedTask;
            }
        }
    }
}
