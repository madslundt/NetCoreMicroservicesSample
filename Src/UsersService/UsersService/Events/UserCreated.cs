using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using UsersService.Infrastructure.Event;

namespace UsersService.Events
{
    public class UserCreated
    {
        public class UserCreatedEvent : IEvent
        {
            public Guid UserId { get; }

            public UserCreatedEvent(Guid userId)
            {
                UserId = userId;
            }
        }

        public class Handler : IEventHandler<UserCreatedEvent>
        {
            public Task Handle(UserCreatedEvent @event, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}
