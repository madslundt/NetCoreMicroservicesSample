using DataModel.Models.Rating;
using FluentValidation;
using Infrastructure.Core.Commands;
using Infrastructure.EventStores.Repository;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReviewsService.Commands
{
    public class CreateReviewCommand
    {
        public class Command : ICommand<Result>
        {
            public Guid UserId { get; set; }
            public Guid MovieId { get; set; }
            public string Text { get; set; }
            public RatingEnum Rating { get; set; }
        }

        public class Result
        {
            public Guid Id { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(cmd => cmd.Text).NotEmpty();
                RuleFor(cmd => cmd.Rating).IsInEnum();
            }
        }

        public class Handler : ICommandHandler<Command, Result>
        {
            private readonly IRepository<ReviewAggregate> _repository;

            public Handler(IRepository<ReviewAggregate> repository)
            {
                _repository = repository;
            }

            public async Task<Result> Handle(Command command, CancellationToken cancellationToken)
            {
                var review = ReviewAggregate.CreateReview(command.UserId, command.MovieId, command.Text, command.Rating);

                await _repository.Add(review);

                var result = new Result
                {
                    Id = review.Id
                };

                return result;
            }
        }
    }
}
