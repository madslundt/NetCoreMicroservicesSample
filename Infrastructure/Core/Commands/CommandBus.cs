using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Core.Commands
{
    public class CommandBus : ICommandBus
    {
        private readonly IMediator _mediator;

        public CommandBus(IMediator mediator)
        {
            _mediator = mediator ?? throw new Exception($"Missing dependency '{nameof(IMediator)}'");
        }

        public virtual async Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command);

            return result;
        }

        public virtual async Task Send(ICommand command, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(command);
        }
    }
}
