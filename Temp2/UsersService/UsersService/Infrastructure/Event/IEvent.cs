using MediatR;

namespace UsersService.Infrastructure.Event
{
    public interface IEvent : INotification
    {}

    public interface IEventHandler<T> : IRequest<T> where T : IEvent
    {}
}
