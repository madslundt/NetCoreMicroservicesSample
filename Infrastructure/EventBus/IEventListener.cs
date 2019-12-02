using System.Threading.Tasks;

namespace Infrastructure.EventBus
{
    public interface IEventListener
    {
        void Subscribe<T>() where T : IEvent;
        Task Publish<T>(T @event) where T : IEvent;
        Task Publish(string message, string type);
    }
}
