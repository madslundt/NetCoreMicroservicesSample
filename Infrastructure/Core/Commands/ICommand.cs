using MediatR;

namespace Infrastructure.Core.Commands
{
    public interface ICommand<out TResponse> : IRequest<TResponse>
    { }

    public interface ICommand : IRequest
    { }
}
