using Convey.CQRS.Commands;
using DataModel;
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
            private readonly DatabaseContext _db;

            public CreateUserMessageHandler(DatabaseContext db)
            {
                _db = db;
            }
            public async Task HandleAsync(Command command)
            {
                var message = new Message
                {
                    Id = command.MessageId,
                    UserId = command.UserId,
                    Text = command.Message
                };

                _db.Add(message);
                await _db.SaveChangesAsync();
            }
        }
    }
}
