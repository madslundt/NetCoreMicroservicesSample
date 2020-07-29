using System;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Core;
using Infrastructure.Outbox;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UsersService.Commands;
using UsersService.Queries;

namespace UsersService.Controllers
{
    [Route("api/users")]
    public class UserController : BaseController
    {
        public UserController(IMediator mediator, IOutboxListener outboxListener, TransactionId transactionId) : base(mediator, outboxListener, transactionId)
        {

        }
        [HttpGet, Route("{id}")]
        public async Task<ActionResult<GetUserQuery.Result>> GetUser([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var query = new GetUserQuery.Query(id);

            var result = await Send(query, cancellationToken);

            return Ok(result);
        }

        [HttpPost, Route("")]
        public async Task<ActionResult<CreateUserCommand.Result>> CreateUser([FromBody] CreateUserCommand.Command user, CancellationToken cancellationToken)
        {
            var result = await Send(user, cancellationToken);

            return Ok(result);
        }
    }
}