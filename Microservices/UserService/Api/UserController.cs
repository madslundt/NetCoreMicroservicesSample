using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Api
{
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet, Route("{userId}")]
        public async Task<IActionResult> GetUser([FromQuery] Guid userId)
        {
            //var query = new GetU

            return Ok();
        }
    }
}
