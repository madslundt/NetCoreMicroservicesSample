using System;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Core.Commands;
using Infrastructure.Core.Queries;
using Microsoft.AspNetCore.Mvc;
using UsersService.Commands;
using UsersService.Queries;

namespace UsersService.Controllers
{
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;

        public UserController(IQueryBus queryBus, ICommandBus commandBus)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
        }

        [HttpGet, Route("{id}")]
        public async Task<ActionResult<GetUserQuery.Result>> GetUser([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var query = new GetUserQuery.Query
            {
                Id = id
            };

            var result = await _queryBus.Send(query, cancellationToken);

            return Ok(result);
        }

        [HttpDelete, Route("{id}")]
        public async Task<ActionResult<GetUserQuery.Result>> DeleteUser([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteUserCommand.Command
            {
                Id = id
            };

            await _commandBus.Send(command, cancellationToken);

            return Ok();
        }

        [HttpPost, Route("")]
        public async Task<ActionResult<CreateUserCommand.Result>> CreateUser([FromBody] CreateUserCommand.Command user, CancellationToken cancellationToken)
        {
            var result = await _commandBus.Send(user, cancellationToken);

            return Ok(result);
        }

        [HttpGet, Route("")]
        public async Task<ActionResult<GetUsersQuery.Result>> CreateUser(CancellationToken cancellationToken)
        {
            var query = new GetUsersQuery.Query
            {
            };

            var result = await _queryBus.Send(query, cancellationToken);

            return Ok(result);
        }
    }
}