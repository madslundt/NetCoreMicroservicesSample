using Infrastructure.EventBus;
using Infrastructure.Outbox;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Infrastructure.Core
{
    public abstract class BaseController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IOutboxListener _outboxListener;
        private readonly TransactionId _transactionId;

        public BaseController(IMediator mediator, IOutboxListener outboxListener, TransactionId transactionId)
        {
            _mediator = mediator;
            _outboxListener = outboxListener;
            _transactionId = transactionId;
        }

        protected async Task<TResult> Send<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);

            return result;
        }

        protected async Task<TResult> Send<TResult>(ICommand<TResult> command, CancellationToken cancellationToken)
        {
            TResult result;

            try
            {
                result = await _mediator.Send(command, cancellationToken);
            }
            catch
            {
                await _outboxListener?.Remove(_transactionId.Value);
                throw;
            }

            await _outboxListener?.Commit(_transactionId.Value);


            return result;
        }
    }
}
