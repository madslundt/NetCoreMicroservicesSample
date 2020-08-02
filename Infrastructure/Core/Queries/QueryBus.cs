using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Core.Queries
{
    public class QueryBus : IQueryBus
    {
        private readonly IMediator _mediator;

        public QueryBus(IMediator mediator)
        {
            _mediator = mediator ?? throw new Exception($"Missing dependency '{nameof(IMediator)}'");   
        }

        public virtual async Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(query);

            return result;
        }
    }
}
