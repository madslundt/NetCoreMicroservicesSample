using FluentValidation;
using Infrastructure.Core.Commands;
using Infrastructure.EventStores.Repository;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MessagesService.Commands
{
    public class CreateMessageCommand
    {
        public class Command : ICommand<Result>
        {
            public Guid UserId { get; }
            public string Text { get; }

            public Command(Guid userId, string text)
            {
                UserId = userId;
                Text = text;
            }
        }

        public class Result
        {
            public Guid Id { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(cmd => cmd.UserId).NotEmpty();
                RuleFor(cmd => cmd.Text).NotEmpty();
            }
        }

        public class Handler : ICommandHandler<Command, Result>
        {
            private readonly IRepository<MessageAggregate> _repository;

            public Handler(IRepository<MessageAggregate> repository)
            {
                _repository = repository;
            }

            public async Task<Result> Handle(Command command, CancellationToken cancellationToken)
            {
                var message = MessageAggregate.CreateMessage(command.UserId, command.Text);

                await _repository.Add(message);

                var result = new Result
                {
                    Id = message.MessageId
                };

                return result;
            }
        }
    }
}
