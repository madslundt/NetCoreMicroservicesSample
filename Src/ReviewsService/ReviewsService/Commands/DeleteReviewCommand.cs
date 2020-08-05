using FluentValidation;
using Infrastructure.Core.Commands;
using Infrastructure.EventStores.Repository;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReviewsService.Commands
{
    public class DeleteReviewCommand
    {
        public class Command : ICommand
        {
            public Guid Id { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(cmd => cmd.Id).NotEmpty();
            }
        }

        public class Handler : ICommandHandler<Command>
        {
            private readonly IRepository<ReviewAggregate> _repository;

            public Handler(IRepository<ReviewAggregate> repository)
            {
                _repository = repository;
            }

            public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
            {
                var review = await _repository.Find(command.Id);

                review.DeleteReview();
                await _repository.Delete(review);

                return Unit.Value;
            }
        }
    }
}
