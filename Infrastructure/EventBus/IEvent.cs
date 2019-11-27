using MediatR;

namespace Infrastructure.EventBus
{
    public interface IEvent : INotification
    { }

    public interface IEventHandler<T> : INotificationHandler<T> where T : IEvent
    { }
}
