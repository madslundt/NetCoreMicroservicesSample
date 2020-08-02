using System.Threading.Tasks;

namespace Infrastructure.Core.Events
{
    public interface IEventBus
    {
        Task Publish(params IEvent[] events);
    }
}
