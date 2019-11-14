using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RawRabbit;
using UsersService.Event;
using UsersService.Queries;

namespace UsersService.Controllers
{
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IBusClient _busClient;

        public UserController(IMediator mediator, IBusClient busClient)
        {
            _mediator = mediator;
            _busClient = busClient;
        }

        [HttpGet, Route("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] Guid id)
        {
            var query = new GetUser.Query(id);

            var result = await _mediator.Send(query);

            await _busClient.PublishAsync(new UserCreated.UserCreatedEvent(id));

            return Ok(result);
        }
    }
}