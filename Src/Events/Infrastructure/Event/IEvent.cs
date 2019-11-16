using MediatR;

namespace Events.Infrastructure.Event
{
    public interface IEvent : INotification
    { }

    public interface IEventHandler<T> : INotificationHandler<T> where T : IEvent
    { }
}
