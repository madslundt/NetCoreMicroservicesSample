using Convey.CQRS.Commands;
using Convey.MessageBrokers;
using DataModel;
using Newtonsoft.Json;
using OpenTracing;
using System;
using System.Threading.Tasks;
using static Users.Service.Events.UserCreated;

namespace Users.Service.Commands
{
    public class CreateUser
    {
        public class Command : ICommand
        {
            [JsonIgnore]
            public Guid UserId { get; }
            public string FirstName { get; }
            public string LastName { get; }

            public Command(string firstName, string lastName)
            {
                UserId = Guid.NewGuid();
                FirstName = firstName;
                LastName = lastName;
            }
        }

        public class CreateUserHandler : ICommandHandler<Command>
        {
            private readonly IBusPublisher _publisher;
            private readonly ITracer _tracer;

            public CreateUserHandler(IBusPublisher publisher, ITracer tracer)
            {
                _publisher = publisher;
                _tracer = tracer;
            }

            public async Task HandleAsync(Command command)
            {
                var user = new User
                {
                    Id = command.UserId,
                    FirstName = command.FirstName,
                    LastName = command.LastName
                };

                var spanContext = _tracer.ActiveSpan.Context.ToString();
                await _publisher.PublishAsync(new UserCreatedEvent(user.Id), spanContext: spanContext);
            }
        }
    }
}
