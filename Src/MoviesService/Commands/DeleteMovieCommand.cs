using FluentValidation;
using Infrastructure.Core.Commands;
using Infrastructure.EventStores.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MoviesService.Commands
{
    public class DeleteMovieCommand
    {
        public class Command : ICommand
        {
            public Guid Id { get; set; }
            public Guid UserId { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(cmd => cmd.Id).NotEmpty();
                RuleFor(cmd => cmd.UserId).NotEmpty();
            }
        }

        public class Handler : ICommandHandler<Command>
        {
            private readonly IRepository<MovieAggregate> _repository;

            public Handler(IRepository<MovieAggregate> repository)
            {
                _repository = repository;
            }
            public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
            {
                var movie = await _repository.Find(command.Id);

                if (movie is null)
                {
                    throw new ArgumentNullException($"Could not find {nameof(movie)} with id '{command.Id}'");
                }

                movie.DeleteMovie(command.UserId);

                await _repository.Delete(movie);


                return Unit.Value;
            }
        }
    }
}
