using Convey.CQRS.Queries;
using System;
using System.Threading.Tasks;

namespace Users.Service.Queries
{
    public class GetUser
    {
        public class Query : IQuery<Result>
        {
            public Guid Id { get; set; }
        }

        public class Result
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public class GetUserHandler : IQueryHandler<Query, Result>
        {
            public Task<Result> HandleAsync(Query query)
            {
                return Task.FromResult(new Result
                {
                    FirstName = "Test",
                    LastName = "Test"
                });
            }
        }
    }
}
