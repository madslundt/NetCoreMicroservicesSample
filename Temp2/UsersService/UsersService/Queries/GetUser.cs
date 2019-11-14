using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace UsersService.Queries
{
    public class GetUser
    {
        public class Query : IRequest<Result>
        {
            public Guid Id { get; }
            public Query(Guid id)
            {
                Id = id;
            }
        }

        public class Result
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
        }

        public class GetUserValidator: AbstractValidator<Query>
        {
            public GetUserValidator()
            {
                RuleFor(query => query.Id).NotEmpty();
            }
        }

        public class GetUserHandler : IRequestHandler<Query, Result>
        {
            public Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                var result = new Result
                {
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Email = "FirstName@LastName.dk"
                };

                return Task.FromResult(result);
            }
        }
    }
}
