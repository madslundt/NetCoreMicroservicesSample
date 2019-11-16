using DataModel;
using DataModel.Models.User;
using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using UsersService.Events;
using UsersService.Infrastructure.RabbitMQ;

namespace UsersService.Commands
{
    public class CreateUser
    {
        public class Command : IRequest<Result>
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
            private readonly RabbitEventListener _rabbitEventListener;
            private readonly DatabaseContext _db;

            public Handler(RabbitEventListener rabbitEventListener, DatabaseContext db)
            {
                _rabbitEventListener = rabbitEventListener;
                _db = db;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var id = await CreateUser(request);

                await _rabbitEventListener.DispatchAsync(new UserCreated.UserCreatedEvent(id)).ConfigureAwait(true);

                var result = new Result
                {
                    Id = id
                };

                return result;
            }

            private async Task<Guid> CreateUser(Command request)
            {
                var user = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email
                };

                _db.Add(user);
                await _db.SaveChangesAsync();

                return user.Id;
            }
        }
    }
}
