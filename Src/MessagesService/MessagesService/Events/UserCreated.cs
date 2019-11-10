using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using Convey.MessageBrokers;
using MessagesService.Commands;
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
            private readonly ICommandDispatcher _commandDispatcher;
            public UserCreatedHandler(ICommandDispatcher commandDispatcher)
            {
                _commandDispatcher = commandDispatcher;
            }
            public Task HandleAsync(UserCreatedEvent @event)
            {
                Console.WriteLine($"Creating message for user {nameof(@event.UserId)} = {@event.UserId}");
                var message = $"Welcome to this awesome stuff";
                _commandDispatcher.SendAsync(new CreateUserMessage.Command(@event.UserId, message));

                return Task.CompletedTask;
            }
        }
    }
}
