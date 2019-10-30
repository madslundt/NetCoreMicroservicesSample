using Convey.CQRS.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessagesService.Commands
{
    public class CreateUserMessage
    {
        public class Command : ICommand
        {
            [JsonIgnore]
            public Guid MessageId { get; }
            public Guid UserId { get; }
            public string Message { get; }

            public Command(Guid userId, string message)
            {
                MessageId = Guid.NewGuid();
                UserId = userId;
                Message = message;
            }
        }

        public class CreateUserMessageHandler : ICommandHandler<Command>
        {
            public async Task HandleAsync(Command command)
            {
                
            }
        }
    }
}
