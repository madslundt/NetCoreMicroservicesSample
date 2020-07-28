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
    public class GetUserMessagesQuery
    {
        public class Query : IRequest<Result>
        {
            public Guid UserId { get; }

            public Query(Guid userId)
            {
                UserId = userId;
            }
        }

        public class Result
        {
            public IEnumerable<Message> Messages { get; set; }
        }

        public class Message
        {
            public Guid Id { get; set; }
            public string Text { get; set; }
            public DateTime Created { get; set; }
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(query => query.UserId).NotEmpty();
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
                var messages = await GetMessages(request.UserId);

                if (messages is null)
                {
                    throw new ArgumentNullException($"{nameof(messages)} was not found");
                }

                var result = new Result
                {
                    Messages = messages
                };

                return result;
            }

            private async Task<IEnumerable<Message>> GetMessages(Guid userId)
            {
                var query = from message in _db.Messages
                            where message.UserId == userId
                            select new Message
                            {
                                Id = message.Id,
                                Text = message.Text,
                                Created = message.Created
                            };

                var result = await query.ToListAsync();

                return result;
            }
        }
    }
}
