using DataModel;
using DataModel.Models.User;
using Events.Users;
using FluentValidation;
using Infrastructure.Core;
using Infrastructure.Core.Commands;
using Infrastructure.Core.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace UsersService.Commands
{
    public class CreateUserCommand
    {
        public class Command : ICommand<Result>
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
        }

        public class Result
        {
            public Guid Id { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(cmd => cmd.FirstName).NotEmpty();
                RuleFor(cmd => cmd.LastName).NotEmpty();
                RuleFor(cmd => cmd.Email).NotEmpty().EmailAddress();
            }
        }

        public class Handler : ICommandHandler<Command, Result>
        {
            private readonly DatabaseContext _db;
            private readonly IEventBus _eventBus;

            public Handler(DatabaseContext db, IEventBus eventBus)
            {
                _db = db;
                _eventBus = eventBus;
            }

            public async Task<Result> Handle(Command command, CancellationToken cancellationToken)
            {
                var user = new User
                {
                    FirstName = command.FirstName,
                    LastName = command.LastName,
                    Email = command.Email
                };

                await _db.AddAsync(user, cancellationToken);

                var @event = Mapping.Map<User, UserCreatedEvent>(user);
                @event.UserId = user.Id;

                await _db.SaveChangesAndCommit(@event);

                var result = new Result
                {
                    Id = user.Id
                };

                return result;
            }
        }
    }
}
