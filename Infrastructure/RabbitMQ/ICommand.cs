using MediatR;

namespace Infrastructure.RabbitMQ
{
    public interface ICommand : IRequest
    { }

    public interface ICommandHandler<T> : IRequestHandler<T> where T : ICommand
    { }
}
