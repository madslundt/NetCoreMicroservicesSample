using DataModel;
using DataModel.Models;
using FluentValidation;
using Infrastructure.Core.Queries;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MoviesService.Queries
{
    public class GetMoviesByUserIdQuery
    {
        public class Query : IQuery<Result>
        {
            public Guid UserId { get; set; }
        }

        public class Result
        {
            public ICollection<MovieItem> Movies { get; set; }
        }

        public class MovieItem
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public int Year { get; set; }
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
                var movies = await GetMovies(query.UserId);

                var result = new Result
                {
                    Movies = movies
                };

                return result;
            }

            private async Task<ICollection<MovieItem>> GetMovies(Guid userId)
            {
                var query = from movie in _db.Movies
                            where movie.UserId == userId
                            select new MovieItem
                            {
                                Id = movie.Id,
                                Title = movie.Title,
                                Year = movie.Year
                            };

                var result = await query.ToListAsync();

                return result;
            }
        }
    }
}
