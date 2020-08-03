using FluentValidation;
using Infrastructure.Core.Commands;
using Infrastructure.EventStores.Repository;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace UsersService.Commands
{
    public class DeleteUserCommand
    {
        public class Command : ICommand
        {
            public Guid Id { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(cmd => cmd.Id).NotEmpty();
            }
        }

        public class Handler : ICommandHandler<Command>
        {
            private readonly IRepository<UserAggregate> _repository;

            public Handler(IRepository<UserAggregate> repository)
            {
                _repository = repository;
            }
            public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
            {
                var user = await _repository.Find(command.Id);

                user.DeleteUser();
                await _repository.Delete(user);


                return Unit.Value;   
            }
        }
    }
}
