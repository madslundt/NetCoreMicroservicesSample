using MediatR;
using UsersService.Infrastructure.Event;

namespace UsersService.Infrastructure.Mediator
{
    public static class MediatorExtensions
    {
        public static void Dispatch(this IMediator mediator, IEvent @event)
        {
            mediator.Publish(@event);
        }
    }
}
