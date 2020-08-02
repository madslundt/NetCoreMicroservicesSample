using Infrastructure.Core.Commands;
using Infrastructure.Core.Queries;
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
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;

        public MessageController(IQueryBus queryBus, ICommandBus commandBus)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
        }

        [HttpGet, Route("messages/{id}")]
        public async Task<ActionResult<GetMessageQuery.Result>> GetMessage([FromRoute] Guid id)
        {
            var query = new GetMessageQuery.Query(id);

            var result = await _queryBus.Send(query);

            return Ok(result);
        }

        [HttpGet, Route("users/{userId}/messages")]
        public async Task<ActionResult<GetUserMessagesQuery.Result>> GetUserMessages([FromRoute] Guid userId)
        {
            var query = new GetUserMessagesQuery.Query(userId);

            var result = await _queryBus.Send(query);

            return Ok(result);
        }

        public class CreateUserMessageRequest
        {
            public string Text { get; set; }
        }
        [HttpPost, Route("users/{userId}/messages")]
        public async Task<ActionResult<CreateMessageCommand.Result>> CreateUserMessage([FromRoute] Guid userId, [FromBody] CreateUserMessageRequest request)
        {
            var command = new CreateMessageCommand.Command(userId, request?.Text);

            var result = await _commandBus.Send(command);

            return Ok(result);
        }
    }
}
