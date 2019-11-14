using System.Threading.Tasks;

namespace UsersService.Infrastructure.Event
{
    //Command – can be subscribed & processed only by a single consumer, produces Event.
    public interface ICommand
    {
    }

    //Event – can be subscribed & processed by one or more consumers, may product another Command(saga and that sort of workflows).
    public interface IEvent
    {
    }

    public interface ICommandHandler<in T> where T : ICommand
    {
        Task HandleAsync(T command);
    }

    public interface IEventHandler<in T> where T : IEvent
    {
        Task HandleAsync(T @event);
    }
}
