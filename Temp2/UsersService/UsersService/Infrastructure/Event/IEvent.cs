using MediatR;

namespace UsersService.Infrastructure.Event
{
    public interface IEvent : INotification
    {}

    public interface IEventHandler<T> : INotificationHandler<T> where T : IEvent
    {}
}
