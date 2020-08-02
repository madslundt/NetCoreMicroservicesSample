using Infrastructure.Core.Events;
using System;
using System.Threading.Tasks;

namespace Infrastructure.MessageBrokers
{
    public interface IEventListener
    {
        void Subscribe(Type type);
        void Subscribe<T>() where T : IEvent;
        Task Publish<T>(T @event) where T : IEvent;
        Task Publish(string message, string type);
    }
}
