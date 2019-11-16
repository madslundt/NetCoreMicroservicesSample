using DataModel;
using DataModel.Models.Message;
using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MessagesService.Commands
{
    public class CreateMessage
    {
        public class Command : IRequest<Result>
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

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly DatabaseContext _db;

            public Handler(DatabaseContext db)
            {
                _db = db;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var id = await CreateMessage(request.UserId, request.Text);

                var result = new Result
                {
                    Id = id
                };

                return result;
            }

            private async Task<Guid> CreateMessage(Guid userId, string text)
            {
                var message = new Message
                {
                    UserId = userId,
                    Text = text
                };

                _db.Add(message);
                await _db.SaveChangesAsync();

                return message.Id;
            }
        }
    }
}
