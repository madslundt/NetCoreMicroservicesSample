using DataModel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MessagesService.Queries
{
    public class GetMessage
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
            public Guid Id { get; set; }
            public string Text { get; set; }
            public Guid UserId { get; set; }
            public DateTime Created { get; set; }
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(query => query.Id).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly DatabaseContext _db;

            public Handler(DatabaseContext db)
            {
                _db = db;
            }
            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                var message = await GetMessage(request.Id);

                if (message is null)
                {
                    throw new ArgumentNullException($"{nameof(message)} was not found");
                }

                return message;
            }

            private async Task<Result> GetMessage(Guid id)
            {
                var query = from message in _db.Messages
                            where message.Id == id
                            select new Result
                            {
                                Id = message.Id,
                                UserId = message.UserId,
                                Text = message.Text,
                                Created = message.Created
                            };

                var result = await query.FirstOrDefaultAsync();

                return result;
            }
        }
    }
}
