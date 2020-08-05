using Infrastructure.Core;
using Infrastructure.Core.Commands;
using Infrastructure.Core.Queries;
using Microsoft.AspNetCore.Mvc;
using ReviewsService.Commands;
using ReviewsService.Dtos;
using ReviewsService.Queries;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReviewsService.Controllers
{
    [Route("api/reviews")]
    public class ReviewController : Controller
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;

        public ReviewController(IQueryBus queryBus, ICommandBus commandBus)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
        }

        [HttpGet, Route("{id}")]
        public async Task<ActionResult<GetReviewQuery.Result>> GetReview([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var query = new GetReviewQuery.Query
            {
                Id = id
            };

            var result = await _queryBus.Send(query, cancellationToken);

            return Ok(result);
        }

        [HttpDelete, Route("{id}")]
        public async Task<IActionResult> DeleteReview([FromRoute] Guid id, [FromHeader] Guid userId, CancellationToken cancellationToken)
        {
            var command = new DeleteReviewCommand.Command
            {
                Id = id
            };

            await _commandBus.Send(command, cancellationToken);

            return Ok();
        }

        [HttpPost, Route("")]
        public async Task<ActionResult<CreateReviewCommand.Result>> CreateMovie([FromBody] CreateReviewDto review, [FromHeader] Guid userId, CancellationToken cancellationToken)
        {
            var command = Mapping.Map<CreateReviewDto, CreateReviewCommand.Command>(review);
            command.UserId = userId;

            var result = await _commandBus.Send(command, cancellationToken);

            return Created("{id}", new { id = result.Id });
        }
    }
}
