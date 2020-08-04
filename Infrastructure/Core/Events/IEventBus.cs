using Infrastructure.EventStores;
using System.Threading.Tasks;

namespace Infrastructure.Core.Events
{
    public interface IEventBus
    {
        Task Commit(params IEvent[] events);
        Task Commit(StreamState stream);
    }
}
