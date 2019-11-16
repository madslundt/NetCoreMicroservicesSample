using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using UsersService.Infrastructure.Event;

namespace UsersService.Events
{
    public class UserDeleted
    {
        public class UserDeletedEvent : IEvent
        {
            public Guid UserId { get; }

            public UserDeletedEvent(Guid userId)
            {
                UserId = userId;
            }
        }

        public class Handler : IEventHandler<UserDeletedEvent>
        {
            public Task Handle(UserDeletedEvent @event, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}
