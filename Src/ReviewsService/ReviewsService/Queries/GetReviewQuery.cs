using DataModel;
using DataModel.Models.Rating;
using FluentValidation;
using Infrastructure.Core.Queries;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReviewsService.Queries
{
    public class GetReviewQuery
    {
        public class Query : IQuery<Result>
        {
            public Guid Id { get; set; }
        }

        public class Result
        {
            public Guid UserId { get; set; }
            public Guid MovieId { get; set; }
            public string Text { get; set; }
            public RatingEnum Rating { get; set; }
            public DateTime CreatedUtc { get; set; }
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(cmd => cmd.Id).NotEmpty();
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
                var review = await GetReview(query.Id);

                if (review is null)
                {
                    throw new ArgumentNullException($"Could not find {nameof(review)} with id '{query.Id}'");
                }

                return review;
            }

            private async Task<Result> GetReview(Guid id)
            {
                var query = from review in _db.Reviews
                            where review.Id == id
                            select new Result
                            {
                                UserId = review.UserId,
                                MovieId = review.MovieId,
                                Text = review.Text,
                                Rating = review.Rating,
                                CreatedUtc = review.CreatedUtc
                            };

                var result = await query.FirstOrDefaultAsync();

                return result;
            }
        }
    }
}
