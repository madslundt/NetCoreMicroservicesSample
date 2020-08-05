using DataModel;
using FluentValidation;
using Infrastructure.Core.Commands;
using Infrastructure.EventStores.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReviewsService.Commands
{
    public class DeleteReviewsByMovieIdCommand
    {
        public class Command : ICommand
        {
            public Guid MovieId { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(cmd => cmd.MovieId).NotEmpty();
            }
        }

        public class Handler : ICommandHandler<Command>
        {
            private readonly IRepository<ReviewAggregate> _repository;
            private readonly DatabaseContext _db;

            public Handler(IRepository<ReviewAggregate> repository, DatabaseContext db)
            {
                _repository = repository;
                _db = db;
            }

            public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
            {
                var reviewIds = await GetReviewIds(command.MovieId);

                var reviews = await _repository.Find(reviewIds);

                foreach (var review in reviews)
                {
                    review.DeleteReview();
                }

                await _repository.Delete(reviews);

                return Unit.Value;
            }

            private async Task<ICollection<Guid>> GetReviewIds(Guid movieId)
            {
                var query = from review in _db.Reviews
                            where review.MovieId == movieId
                            select review.Id;

                var result = await query.ToListAsync();

                return result;
            }
        }
    }
}
