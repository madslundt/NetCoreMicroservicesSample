using Infrastructure.Core.Events;
using System.Threading.Tasks;

namespace Infrastructure.Outbox
{
    public interface IOutboxListener
    {
        Task Commit(OutboxMessage message);
        Task Commit<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}
