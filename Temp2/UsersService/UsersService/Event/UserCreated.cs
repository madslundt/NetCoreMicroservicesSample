using MediatR;
using System;
using System.Threading.Tasks;
using UsersService.Infrastructure.Event;

namespace UsersService.Event
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
            public Handler(IMediator mediator)
            {

            }
            public Task HandleAsync(UserCreatedEvent @event)
            {
                return Task.CompletedTask;
            }
        }
    }
}
