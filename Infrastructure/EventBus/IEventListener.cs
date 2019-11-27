using System.Threading.Tasks;

namespace Infrastructure.EventBus
{
    public interface IEventListener
    {
        void SubscribeEvent<T>() where T : IEvent;
        Task Publish<T>(T @event) where T : IEvent;
        Task Publish(string message, string type);

        void SubscribeCommand<T>() where T : ICommand;
        Task Send<T>(T command) where T : ICommand;
        Task Send(string message, string type);
    }
}
