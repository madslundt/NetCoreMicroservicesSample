using DataModel;
using FluentValidation;
using Infrastructure.Core.Queries;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UsersService.Queries
{
    public class GetUserQuery
    {
        public class Query : IQuery<Result>
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

        public class Validator: AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(query => query.Id).NotEmpty();
            }
        }

        public class Handler : IQueryHandler<Query, Result>
        {
            private readonly DatabaseContext _db;

            public Handler(DatabaseContext db)
            {
                _db = db;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await GetUser(request.Id);

                if (user is null)
                {
                    throw new ArgumentNullException($"{nameof(user)} was not found");
                }

                return user;
            }

            private async Task<Result> GetUser(Guid id)
            {
                var query = from user in _db.Users
                            where user.Id == id
                            select new Result
                            {
                                Email = user.Email,
                                FirstName = user.FirstName,
                                LastName = user.LastName
                            };

                var result = await query.FirstOrDefaultAsync();

                return result;
            }
        }
    }
}
