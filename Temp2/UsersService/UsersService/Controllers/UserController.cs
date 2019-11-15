using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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
            var query = new GetUser.Query(id);

            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}