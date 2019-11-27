using MediatR;

namespace Infrastructure.EventBus
{
    public interface ICommand : IRequest
    { }

    public interface ICommandHandler<T> : IRequestHandler<T> where T : ICommand
    { }
}
