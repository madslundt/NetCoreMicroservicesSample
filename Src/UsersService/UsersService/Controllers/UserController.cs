using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UsersService.Commands;
using UsersService.Queries;

namespace UsersService.Controllers
{
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet, Route("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] Guid id)
        {
            var query = new GetUserQuery.Query(id);

            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpPost, Route("")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand.Command user)
        {
            var result = await _mediator.Send(user);

            return Ok(result);
        }
    }
}