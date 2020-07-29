using DataModel.Models.User;
using FluentValidation;
using Infrastructure.EventBus;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using UsersService.Repository;

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

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly IUserRepository _repository;

            public Handler(IUserRepository repository)
            {
                _repository = repository;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email
                };

                await _repository.CreateUser(user, cancellationToken);

                var result = new Result
                {
                    Id = user.Id
                };

                return result;
            }
        }
    }
}
