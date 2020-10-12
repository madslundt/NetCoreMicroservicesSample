using DataModel;
using FluentValidation;
using Infrastructure.Core.Queries;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace UsersService.Queries
{
    public class GetUsersQuery
    {
        public class Query : IQuery<Result>
        {
        }

        public class User
        {
            public Guid Id { get; set; } 
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
        }

        public class Result
        {
            public List<User> Users { get; set; }
        }

        public class Validator: AbstractValidator<Query>
        {
            public Validator()
            {
            }
        }

        public class Handler : IQueryHandler<Query, Result>
        {
            private readonly DatabaseContext _db;

            public Handler(DatabaseContext db)
            {
                _db = db;
            }

            public async Task<Result> Handle(Query query, CancellationToken cancellationToken)
            {
                var users = await GetUsers();

                return users;
            }

            private async Task<Result> GetUsers()
            {
                var query = from user in _db.Users
                            select new User
                            {
                                Id = user.Id,
                                Email = user.Email,
                                FirstName = user.FirstName,
                                LastName = user.LastName
                            };

                var result = new Result 
                { 
                    Users = await query.ToListAsync() 
                };

                return result;
            }
        }
    }
}
