using FluentValidation;
using Infrastructure.Core.Commands;
using Infrastructure.EventStores.Repository;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MoviesService.Commands
{
    public class CreateMovieCommand
    {
        public class Command : ICommand<Result>
        {
            public string Title { get; set; }
            public int Year { get; set; }
            public Guid UserId { get; set; }
        }

        public class Result
        {
            public Guid Id { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(cmd => cmd.Title).NotEmpty();
                RuleFor(cmd => cmd.UserId).NotEmpty();
                RuleFor(cmd => cmd.Year).GreaterThanOrEqualTo(1900);
            }
        }

        public class Handler : ICommandHandler<Command, Result>
        {
            private readonly IRepository<MovieAggregate> _repository;

            public Handler(IRepository<MovieAggregate> repository)
            {
                _repository = repository;
            }
            public async Task<Result> Handle(Command command, CancellationToken cancellationToken)
            {
                var movie = MovieAggregate.CreateMovie(command.Title, command.Year, command.UserId);

                await _repository.Add(movie);

                var result = new Result
                {
                    Id = movie.Id
                };

                return result;
            }
        }
    }
}
