using Infrastructure.Core;
using Infrastructure.Core.Commands;
using Infrastructure.Core.Queries;
using Microsoft.AspNetCore.Mvc;
using MoviesService.Commands;
using MoviesService.Dtos;
using MoviesService.Queries;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MoviesService.Controllers
{
    [Route("api/movies")]
    public class MovieController : Controller
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;

        public MovieController(IQueryBus queryBus, ICommandBus commandBus)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
        }

        [HttpGet, Route("{id}")]
        public async Task<ActionResult<GetMovieQuery.Result>> GetMovie([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var query = new GetMovieQuery.Query
            {
                Id = id
            };

            var result = await _queryBus.Send(query, cancellationToken);

            return Ok(result);
        }

        [HttpDelete, Route("{id}")]
        public async Task<IActionResult> DeleteMovie([FromRoute] Guid id, [FromHeader] Guid userId, CancellationToken cancellationToken)
        {
            var command = new DeleteMovieCommand.Command
            {
                Id = id,
                UserId = userId
            };

            await _commandBus.Send(command, cancellationToken);

            return Ok();
        }

        [HttpPost, Route("")]
        public async Task<ActionResult<CreateMovieCommand.Result>> CreateMovie([FromBody] CreateMovieDto movie, [FromHeader] Guid userId, CancellationToken cancellationToken)
        {
            var command = Mapping.Map<CreateMovieDto, CreateMovieCommand.Command>(movie);
            command.UserId = userId;

            var result = await _commandBus.Send(command, cancellationToken);

            return Created("{id}", new { id = result.Id });
        }
    }
}