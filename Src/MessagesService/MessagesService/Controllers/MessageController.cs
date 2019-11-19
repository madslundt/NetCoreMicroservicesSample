using MediatR;
using MessagesService.Commands;
using MessagesService.Queries;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MessagesService.Controllers
{
    [Route("api")]
    public class MessageController : Controller
    {
        private readonly IMediator _mediator;

        public MessageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet, Route("messages/{id}")]
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

        public class CreateUserMessageRequest
        {
            public string Text { get; set; }
        }
        [HttpPost, Route("users/{userId}/messages")]
        public async Task<IActionResult> CreateUserMessage([FromRoute] Guid userId, [FromBody] CreateUserMessageRequest request)
        {
            var command = new CreateMessage.Command(userId, request?.Text);

            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}
