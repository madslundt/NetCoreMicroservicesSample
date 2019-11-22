using MediatR;

namespace Infrastructure.RabbitMQ
{
    public interface IEvent : INotification
    { }

    public interface IEventHandler<T> : INotificationHandler<T> where T : IEvent
    { }
}
