using DataModel;
using FluentValidation;
using Infrastructure.Core.Queries;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MoviesService.Queries
{
    public class GetMovieQuery
    {
        public class Query : IQuery<Result>
        {
            public Guid Id { get; set; }
        }

        public class Result
        {
            public string Title { get; set; }
            public int Year { get; set; }
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
                var movie = await GetMovie(query.Id);

                if (movie is null)
                {
                    throw new ArgumentNullException($"Could not find {nameof(movie)} with id '{query.Id}'");
                }

                return movie;
            }

            private async Task<Result> GetMovie(Guid id)
            {
                var query = from movie in _db.Movies
                            where movie.Id == id
                            select new Result
                            {
                                Title = movie.Title,
                                Year = movie.Year,
                                CreatedUtc = movie.CreatedUtc
                            };

                var result = await query.FirstOrDefaultAsync();

                return result;
            }
        }
    }
}
