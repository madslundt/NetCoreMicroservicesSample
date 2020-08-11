using DataModel;
using DataModel.Models.Rating;
using FluentValidation;
using Infrastructure.Core.Queries;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReviewsService.Queries
{
    public class GetReviewsByUserIdQuery
    {
        public class Query : IQuery<Result>
        {
            public Guid UserId { get; set; }
        }

        public class Result
        {
            public ICollection<ReviewItem> Reviews { get; set; }
        }

        public class ReviewItem
        {
            public Guid Id { get; set; }
            public Guid MovieId { get; set; }
            public string Text { get; set; }
            public RatingEnum Rating { get; set; }
            public DateTime CreatedUtc { get; set; }
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(cmd => cmd.UserId).NotEmpty();
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
                var reviews = await GetReviews(query.UserId);

                var result = new Result
                {
                    Reviews = reviews
                };

                return result;
            }

            private async Task<ICollection<ReviewItem>> GetReviews(Guid userId)
            {
                var query = from review in _db.Reviews
                            where review.UserId == userId
                            select new ReviewItem
                            {
                                Id = review.Id,
                                MovieId = review.MovieId,
                                Text = review.Text,
                                Rating = review.Rating,
                                CreatedUtc = review.CreatedUtc
                            };

                var result = await query.ToListAsync();

                return result;
            }
        }
    }
}
