using Convey.CQRS.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessagesService.Queries
{
    public class GetUserMessages
    {
        public class Query : IQuery<Result>
        {
            public Guid UserId { get; set; }
        }

        public class Result
        {
            public ICollection<Message> Messages { get; set; }
        }

        public class Message
        {
            public Guid Id { get; set; }
            public string Text { get; set; }
        }

        public class GetUserHandler : IQueryHandler<Query, Result>
        {
            public Task<Result> HandleAsync(Query query)
            {
                var messages = new List<Message>();

                return Task.FromResult(new Result
                {
                    Messages = messages
                });
            }
        }
    }
}
