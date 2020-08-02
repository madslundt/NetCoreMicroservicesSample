using MediatR;

namespace Infrastructure.Core.Events
{
    public interface IEventHandler<T> : INotificationHandler<T> where T : IEvent
    { }
}
