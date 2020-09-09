using Infrastructure.Core.Events;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Infrastructure.MessageBrokers.Dapr
{
    [Route("dapr/messages")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MessageController : ControllerBase
    {
        private readonly IEventBus _eventBus;

        public MessageController(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [HttpPost, Route("")]
        public async Task<IActionResult> ReceiveMessage([FromBody] Message message)
        {
            await _eventBus.PublishLocal(message.Content);

            return Ok();
        }
    }
}
