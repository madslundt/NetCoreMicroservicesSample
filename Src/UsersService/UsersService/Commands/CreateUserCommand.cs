using Events.Users;
using FluentValidation;
using Infrastructure.Core.Commands;
using Infrastructure.EventStores.Repository;
using Infrastructure.Outbox;
using MediatR;
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
            private readonly IRepository<UserAggregate> _repository;

            public Handler(IRepository<UserAggregate> repository)
            {
                _repository = repository;
            }
            public async Task<Result> Handle(Command command, CancellationToken cancellationToken)
            {
                var user = UserAggregate.CreateUser(command.FirstName, command.LastName, command.Email);

                await _repository.Add(user);

                var result = new Result
                {
                    Id = user.UserId
                };

                return result;
            }
        }
    }
}
