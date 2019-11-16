using MediatR;
using MessagesService.Queries;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MessagesService.Controllers
{
    [Route("api/messages")]
    public class MessageController : Controller
    {
        private readonly IMediator _mediator;

        public MessageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet, Route("{id}")]
        public async Task<IActionResult> GetMessage([FromRoute] Guid id)
        {
            var query = new GetMessage.Query(id);

            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpGet, Route("users/{userId}/messages")]
        public async Task<IActionResult> GetUserMessages([FromRoute] Guid userId)
        {
            var query = new GetUserMessages.Query(userId);

            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}
