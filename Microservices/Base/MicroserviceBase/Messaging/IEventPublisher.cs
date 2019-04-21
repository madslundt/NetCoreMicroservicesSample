using System.Threading.Tasks;

namespace MicroserviceBase.Messaging
{
    public interface IEventPublisher
    {
        Task PublishMessage<T>(T msg);
    }
}
