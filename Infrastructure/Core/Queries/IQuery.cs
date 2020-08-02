using MediatR;

namespace Infrastructure.Core.Queries
{
    public interface IQuery<out TResponse> : IRequest<TResponse>
    { }
}
