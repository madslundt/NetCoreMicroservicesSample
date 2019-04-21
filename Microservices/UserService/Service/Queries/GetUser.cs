using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Queries
{
    public class GetUser
    {
        public class Query : IRequest<Result>
        {
            public Guid Id { get; set; }
        }

        public class Result
        {
            public Guid Id { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime Created { get; set; }
        }

        public class GetUserValidator : AbstractValidator<Query>
        {
            public GetUserValidator()
            {
                RuleFor(user => user.Id).NotEmpty();
            }
        }


        public class GetUserHandler : IRequestHandler<Query, Result>
        {
            public async Task<Result> Handle(Query message, CancellationToken cancellationToken)
            {
                return new Result();
            }
        }
    }
}
